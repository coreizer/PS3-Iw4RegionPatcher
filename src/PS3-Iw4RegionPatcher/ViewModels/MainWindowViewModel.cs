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

namespace PS3_Iw4RegionPatcher.ViewModels
{
   using System;
   using System.IO;
   using System.Threading.Tasks;
   using System.Windows.Input;
   using Microsoft.Win32;
   using ReactiveUI;

   public class MainWindowViewModel : ReactiveObject
   {
      public ICommand OpenFileCommand { get; set; }
      public ICommand ApplyCommand { get; set; }

      private string _filePath;
      public string FilePath
      {
         get {
            return this._filePath;
         }
         set {
            this.RaiseAndSetIfChanged(ref this._filePath, value);
         }
      }

      private string _dialogMessage;
      public string DialogMessage
      {
         get {
            return this._dialogMessage;
         }
         set {
            this.RaiseAndSetIfChanged(ref this._dialogMessage, value);
         }
      }

      private string _selectedItem = "BLUS30377";
      public string SelectedItem
      {
         get {
            return this._selectedItem;
         }
         set {
            this.RaiseAndSetIfChanged(ref this._selectedItem, value);
         }
      }

      private bool _isActivated;
      public bool IsActivated
      {
         get {
            return this._isActivated;
         }
         private set {
            this.RaiseAndSetIfChanged(ref this._isActivated, value);
         }
      }

      public MainWindowViewModel()
      {
         this.OpenFileCommand = ReactiveCommand.CreateFromTask(this.OnOpenFile);
         this.ApplyCommand = ReactiveCommand.CreateFromTask(this.RegionPatcher);
      }

      private async Task OnOpenFile()
      {
         try {
            if (string.IsNullOrWhiteSpace(this.SelectedItem)) {
               throw new InvalidOperationException("No 'Region Code' selected.");
            }

            OpenFileDialog OFD = new OpenFileDialog();
            OFD.Filter = "Fast File (*.ff)|*.ff";
            OFD.FileName = "patch_mp.ff";
            OFD.Multiselect = false;
            if ((bool)OFD.ShowDialog()) {
               this.FilePath = OFD.FileName;
               this.IsActivated = true;
            }
         }
         catch (Exception ex) {
            this.DialogMessage = ex.Message;
            MainWindow.OwnerDialog.Show();
         }
      }

      private async Task RegionPatcher()
      {
         try {
            using (FileStream fileStream = new FileStream(this.FilePath, FileMode.Open, FileAccess.ReadWrite, FileShare.None)) {
               fileStream.Position = 0x18;
               fileStream.WriteByte(this.GetRegionByte(this.SelectedItem));
               await fileStream.FlushAsync();
            }

            this.DialogMessage = "has been successfully applied.";
         }
         catch(Exception ex) {
            this.DialogMessage = ex.Message;
         }
         finally {
            MainWindow.OwnerDialog.Show();
         }
      }

      private byte GetRegionByte(string regionCode)
      {
         switch (regionCode) {
            case "BLES00683":
            case "BLUS30377":
               return 0x01;

            case "BLES00687":
               return 0x10;

            case "BLES00686":
               return 0x04;

            case "BLES00685":
               return 0x08;

            case "BLES00684":
               return 0x02;
         }

         // Default Region
         return 0x01;
      }
   }
}
