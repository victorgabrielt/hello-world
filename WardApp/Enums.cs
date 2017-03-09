using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace WardApp
{
    public abstract class Enums
    {
        public enum AppTheme
        {
            LightTheme,
            DarkTheme
        };

        public enum ReservaStatus
        {          
            Arquivada,
            Ativa
        };
    }
}