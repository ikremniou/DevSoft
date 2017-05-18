﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Net;
using System.Net.Sockets;
using System.Windows.Markup;
using System.Threading;
using System.Windows;
using System.IO;

namespace nGwentCard
{
    public class Connection
    {
        public Battleground battlegnd { get; set; }
        Package ReseivedPackage { get; set; }   
        bool IsConnectionAlive { get; set; }
        NetworkStream stream { get; set; }
        TcpClient client { get; set; }
        Thread receiveThread { get; set; }

        public void InitConnection()
        {
            string Server = ConfigurationManager.AppSettings["ServerIP"];
            int port = Convert.ToInt32(ConfigurationManager.AppSettings["ServerPort"]);
            client = new TcpClient();
            ReseivedPackage = null;           
            try
            {
                ConnectToServer(Server,port);
                this.receiveThread = new Thread(new ThreadStart(GetMessages));
                receiveThread.Start();               
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                battlegnd.EndBattle();
            }
        }

        private void ConnectToServer(string Server, int port)
        {
            client.Connect(Server, port);
            stream = client.GetStream();
        }

        private void GetMessages()
        {
           try
           {
            
                IsConnectionAlive = true;        
                while(IsConnectionAlive)
                {
                    Package pkg = new Package();            
                    StringBuilder builder = new StringBuilder();
                    short Size = 0;
                    byte[] SizeBytes = new byte[2];
                    stream.Read(SizeBytes, 0, 2);                    
                    Size = BitConverter.ToInt16(SizeBytes,0);
                    byte[] bytes = new byte[Size];
                    stream.Read(bytes, 0, bytes.Length);
                    builder.Append(Encoding.Default.GetString(bytes));
                    string str = builder.ToString().TrimEnd('\0');
                    pkg = XamlReader.Parse(str) as Package;
                    ProcessPackage(pkg, battlegnd);
                }
                stream.Close();
                client.Close();
                
            }
            catch (ThreadAbortException ex)
            {
            }
            catch (Exception ex)
            {         
                MessageBox.Show(ex.Message);
                battlegnd.Control.Dispatcher.Invoke(() =>
                {
                    battlegnd.EndBattle();
                });
            }
        }

        public void CloseConnection()
        {
            if (receiveThread != null) receiveThread.Abort();
            if (stream != null) stream.Close();
            if (client != null) client.Close();         
        }

        private void ProcessPackage(Package pkg,Battleground battlegnd)
        {
           
            if (pkg is ICommandable)
            {
                ICommandable Command = pkg as ICommandable;
                ProcessCommand(pkg, Command);
            }
            if (pkg is ISimple)
            {
                ISimple Simple = pkg as ISimple;
                ProcessSimple(pkg, Simple);
            }
        }

        private void ProcessSimple(Package pkg, ISimple Simple)
        {
            
            
            SendGoodCommand();
        }

        private void SendGoodCommand()
        {
            NetCommandPackage GoodPkg = new NetCommandPackage();
            GoodPkg.Command = ConfigurationManager.AppSettings["IsGood"];
            SendMessage(GoodPkg);
        }

        private void ProcessCommand(Package pkg, ICommandable Command)
        {

            if (Command.Command == ConfigurationManager.AppSettings["StartGameCommand"])
            {
                NetCommandPackage SyncPackage = new NetCommandPackage();
                SyncPackage.Command = ConfigurationManager.AppSettings["SyncGameCommand"];
                SyncPackage.InDeckCardCount = battlegnd.InHandCards.Count;
                SyncPackage.InHandCardCount = battlegnd.InStackCards.Count;
                SyncPackage.Scope = battlegnd.UserCardsPower;
                SendMessage(SyncPackage);
            }
            else if (Command.Command == ConfigurationManager.AppSettings["SyncGameCommand"])
            {
                battlegnd.Control.Dispatcher.Invoke(() =>
                {
                    battlegnd.OponentInHandCardCount = pkg.InHandCardCount;
                    battlegnd.OponentStackCardCount = pkg.InDeckCardCount;
                    battlegnd.OponentCardPower = pkg.Scope;
                    battlegnd.Sync(Convert.ToString(pkg.Scope),
                        Convert.ToString(pkg.InDeckCardCount), Convert.ToString(pkg.InHandCardCount));
                });
                SendGoodCommand();
            }
            else if (Command.Command == ConfigurationManager.AppSettings["TurnWaitGameCommand"])
            {
                battlegnd.Control.Dispatcher.Invoke(() =>
                {
                    battlegnd.ShowNotMessage("Ходит ваш опонент, подождите");
                    battlegnd.PlayGroundGrid.IsEnabled = false;
                });
                SendGoodCommand();
            }
            else if (Command.Command == ConfigurationManager.AppSettings["StartTurnGameCommand"])
            {
                battlegnd.Control.Dispatcher.Invoke(() =>
                {
                    battlegnd.ShowNotMessage("Ваш ход");
                    battlegnd.PlayGroundGrid.IsEnabled = true;
                });
                SendGoodCommand();
            }
            else if (Command.Command == ConfigurationManager.AppSettings["ConnectionLost"])
            {
                battlegnd.Control.Dispatcher.Invoke(() =>
                {
                    MessageBox.Show("Потеряно соединение с другим пользователем, перенаправление в меню...");
                    battlegnd.EndBattle();
                });
            }
            else if (Command.Command == ConfigurationManager.AppSettings["LeaveGameCommand"])
            {
                battlegnd.Control.Dispatcher.Invoke(() =>
                {
                    MessageBox.Show("Ваш противник вышел из игры, перенаправление в меню...");
                    battlegnd.EndBattle();
                });
            }
            else if (Command.Command == ConfigurationManager.AppSettings["LeaveGameCommand"])
            {
                battlegnd.Control.Dispatcher.Invoke(() =>
                {
                    battlegnd.Passed();
                });
            }               
        }

        public void SendPassedCommand()
        {
            NetCommandPackage Passed = new NetCommandPackage();
            Passed.Command = ConfigurationManager.AppSettings["SyncGameCommand"];
            SendMessage(Passed);
        }

        public void SendSyncCommand()
        {
            NetCommandPackage Sync = new NetCommandPackage();
            Sync.Command = ConfigurationManager.AppSettings["SyncGameCommand"];
            Sync.InDeckCardCount = battlegnd.InStackCards.Count;
            Sync.Scope = battlegnd.UserCardsPower;
            Sync.InHandCardCount = battlegnd.InHandCards.Count;
            SendMessage(Sync);
        }

        public void SendEndTurnCommand()
        {
            NetCommandPackage TurnEnd = new NetCommandPackage();
            TurnEnd.Command = ConfigurationManager.AppSettings["TurnEndGameCommand"];
            SendMessage(TurnEnd);
        }

        public void SendSimpleCommand(int AffectedCardPos, int CardID, bool IsSpAbilitiPerformed, bool IsRemoved)
        {
            NetSimplePackage Simple = new NetSimplePackage();
            Simple.IsRemoved = IsRemoved;
            Simple.AffectedCardPos = AffectedCardPos;
            Simple.CardID = CardID;
            Simple.IsSpecialAbilitiPerformed = IsSpAbilitiPerformed;
            SendMessage(Simple);
        }

        public void SendLeaveCommand()
        {
            NetCommandPackage Leave = new NetCommandPackage();
            Leave.Command = ConfigurationManager.AppSettings["LeaveGameCommand"];
            SendMessage(Leave);
        }

        private void SendMessage(Package Message)
        {
            string Str = XamlWriter.Save(Message);
            byte[] buff = Encoding.Default.GetBytes(Str);
            short Size = (short)buff.Length;
            byte[] BSize = BitConverter.GetBytes(Size);
            stream.Write(BSize, 0, BSize.Length);
            stream.Write(buff, 0, buff.Length);

        }
    }
}
