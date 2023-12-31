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
using Microsoft.Win32;

namespace Playtesting
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Controller? controller;

        public MainWindow()
        {
            InitializeComponent();
            LoadButton.Click += (_, _) =>
            {
                var fileDialog = new OpenFileDialog
                {
                    Filter = "minden támogatott fájl |*.csv;*.json|szöveges adatok |*.csv|JSON export fájl|*.json|" +
                             "Minden fájl |*.*",
                    Multiselect = false,
                    Title = "Forrás betöltése"
                };
                fileDialog.FileOk += (_, _) =>
                {
                    var isFileJson = fileDialog.SafeFileName.EndsWith(".json");
                    controller = isFileJson
                        ? FileReader.ReadFromJson(fileDialog.OpenFile())
                        : FileReader.ReadFromCsv(fileDialog.OpenFile());
                    SaveButton.IsEnabled = true;
                    DeleteButton.IsEnabled = true;
                    NewTesterButton.IsEnabled = true;
                    controller.SetBindingFor(Grid);
                };
                fileDialog.ShowDialog();
            };
            SaveButton.Click += (_, _) =>
            {
                var fileDialog = new SaveFileDialog
                {
                    Filter = "minden támogatott fájl |*.csv;*.json|szöveges adatok |*.csv|JSON export fájl |*.json|" +
                             "Minden fájl |*.*",
                    Title = "Mentés"
                };
                fileDialog.FileOk += (_, _) =>
                {
                    var isFileJson = fileDialog.SafeFileName.EndsWith(".json");
                    if(isFileJson) controller!.SaveToJson(fileDialog.OpenFile());
                    else controller!.SaveToCsv(fileDialog.OpenFile());
                    MessageBox.Show("Fájl elmentve!","Sikeres mentés!",MessageBoxButton.OK,MessageBoxImage.Information);
                };
                fileDialog.ShowDialog();
            };
            DeleteButton.Click += (_, _) =>
            {
                var answer = MessageBox.Show("Biztosan törli ezt a sort?", "Törlés", MessageBoxButton.YesNo,
                    MessageBoxImage.Warning,
                    MessageBoxResult.No);
                if (answer is MessageBoxResult.No) return;
                if (Grid.SelectedItem is not Tester selected)
                {
                    MessageBox.Show("Nincs sor kijelölve!", "Hiba!", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                var success = controller!.DeleteTester(selected);
                if (success)
                {
                    MessageBox.Show("Sor törölve!", "Sikeres törlés!", MessageBoxButton.OK,
                        MessageBoxImage.Information);
                    controller!.SetBindingFor(Grid);
                }
                else MessageBox.Show("Valamilyen hiba történt a törlés során.", "Hiba!", MessageBoxButton.OK,
                    MessageBoxImage.Error);
            };
            NewTesterButton.Click += (_, _) =>
            {
                var newTesterWindow = new NewTesterWindow(controller!);
                newTesterWindow.Closed += (_, _) =>
                {
                    controller!.SetBindingFor(Grid);
                };
                newTesterWindow.ShowDialog();
            };
        }
    }
}
