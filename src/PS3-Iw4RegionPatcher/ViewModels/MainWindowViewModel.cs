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

using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Microsoft.Win32;
using ReactiveUI;
using Wpf.Ui.Controls;

namespace PS3_Iw4RegionPatcher.ViewModels {
  public class MainWindowViewModel : ReactiveObject {
    public ICommand OpenFileCommand { get; set; }

    public ICommand RegionCommand { get; set; }

    private string _filePath;
    public string FilePath {
      get => this._filePath;
      set => this.RaiseAndSetIfChanged(ref this._filePath, value);
    }

    private string _selectedItem = "BLUS30377";
    public string SelectedItem {
      get => this._selectedItem;
      set => this.RaiseAndSetIfChanged(ref this._selectedItem, value);
    }

    private bool _isActivated;
    public bool IsActivated {
      get => this._isActivated;
      private set => this.RaiseAndSetIfChanged(ref this._isActivated, value);
    }

    public MainWindowViewModel() {
      this.OpenFileCommand = ReactiveCommand.Create(this.OnOpenFile);
      this.RegionCommand = ReactiveCommand.CreateFromTask(this.OnRegion);
    }

    private async void OnOpenFile() {
      try {
        ArgumentNullException.ThrowIfNull(this.SelectedItem);

        var OFD = new OpenFileDialog {
          Filter = "Fast File (*.ff)|*.ff",
          FileName = "patch_mp.ff",
          Multiselect = false
        };

        if (OFD.ShowDialog() == true) {
          this.FilePath = OFD.FileName;
          this.IsActivated = true;
        }
      }
      catch (Exception ex) {
        await new Wpf.Ui.Controls.MessageBox { Title = "Error", CloseButtonAppearance = ControlAppearance.Danger, Content = ex.Message }.ShowDialogAsync();
      }
    }

    private async Task OnRegion() {
      try {
        using var fs = new FileStream(this.FilePath, FileMode.Open, FileAccess.ReadWrite, FileShare.None);
        fs.Position = 0x18;
        fs.WriteByte(GetRegionByte(this.SelectedItem));
        await fs.FlushAsync(); // 書き込み
        await new Wpf.Ui.Controls.MessageBox { Title = "Successfully", CloseButtonAppearance = ControlAppearance.Info, Content = $"Region: {this.SelectedItem}" }.ShowDialogAsync();
      }
      catch (Exception ex) {
        await new Wpf.Ui.Controls.MessageBox { Title = "Error", CloseButtonAppearance = ControlAppearance.Danger, Content = ex.Message }.ShowDialogAsync();
      }
    }

    private static byte GetRegionByte(string regionCode) =>
      regionCode switch {
        "BLES00683" or "BLUS30377" => 0x01,
        "BLES00687" => 0x10,
        "BLES00686" => 0x04,
        "BLES00685" => 0x08,
        "BLES00684" => 0x02,
        _ => 0x01
      };
  }
}
