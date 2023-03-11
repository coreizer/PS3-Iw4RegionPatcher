#region License Information (GPL v3)

/**
 * Copyright (C) 2022 coreizer
 * 
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <https://www.gnu.org/licenses/>.
 */

#endregion

namespace PS3_Iw4RegionPatcher
{
   using System.Windows;
   using PS3_Iw4RegionPatcher.ViewModels;

   /// <summary>
   /// Interaction logic for MainWindow.xaml
   /// </summary>
   public partial class MainWindow
   {
      public static Wpf.Ui.Controls.Dialog OwnerDialog { get; private set; }

      public MainWindow()
      {
         this.InitializeComponent();

         OwnerDialog = this.MainDialog;

         this.DataContext = new MainWindowViewModel();
         Wpf.Ui.Appearance.Background.Apply(this, Wpf.Ui.Appearance.BackgroundType.Mica);
      }

      private void MainDialog_ButtonRightClick(object sender, RoutedEventArgs e)
      {
         (sender as Wpf.Ui.Controls.Dialog).Hide();
      }
   }
}
