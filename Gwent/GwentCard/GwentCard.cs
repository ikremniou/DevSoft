﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace nGwentCard
{
    public class GwentCard
    {       
        public int CardID { get; set; }
        public string CardName { get; set; }
        public string CardDescription { get; set; }
        public int FractionID { get; set; }
        public string FractionName { get; set; }
        public string FractionDescription { get; set; }
        public int SpAbilityID { get; set; }
        public string SpAbilityName { get; set; }
        public string SpAbilityDescription { get; set; }
        public string ToImgPath { get; set; }
        public int CardLine { get; set; }
        public delegate void SpAbilityDelegate();
        public SpAbilityDelegate SpAbilityMethod;
        public Image Image { get; set; }
    }
}