﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.FSharp.Core;

namespace Tuktuk.Wpf.Controls
{
    /// <summary>
    /// WorkspaceControl.xaml の相互作用ロジック
    /// </summary>
    public partial class WorkspaceControl : UserControl
    {
        public WorkspaceControl()
        {
            InitializeComponent();

            GotFocus += (sender, e) =>
            {
                var workspace = DataContext as Workspace;
                if (workspace != null) workspace.GotFocus.OnNext(default(Unit));
            };
        }
    }
}
