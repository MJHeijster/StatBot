﻿using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.WindowsAPICodePack.Dialogs;
using StatBot.Settings;
using StatBotConfiguration.Handlers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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

namespace StatBotConfiguration
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        BotSettings appsettings;
        public IEnumerable<ComboboxValue> FileNameValues { get; set; }
        public MainWindow()
        {
            var builder = CreateConfigurationBuilder();
            IConfigurationRoot configuration = builder.Build();
            SettingsHandler settingsHandler = new SettingsHandler();
            appsettings = settingsHandler.ReadSettings(configuration);
            InitializeComponent();
            SetValues(appsettings);
            DataContext = this;
        }

        private void SetValues(BotSettings appsettings)
        {
            //Discord
            TokenValue.Text = appsettings.Discord.Token;
            DebugChannelIdValue.Text = appsettings.Discord.DebugChannelId;
            CommandPrefixValue.Text = string.IsNullOrEmpty(appsettings.Discord.Commands.Prefix) ? "!" : appsettings.Discord.Commands.Prefix;
            ExcludeCommandValue.Text = string.IsNullOrEmpty(appsettings.Discord.Commands.Exclude) ? "excludefromstats" : appsettings.Discord.Commands.Exclude;
            IncludeCommandValue.Text = string.IsNullOrEmpty(appsettings.Discord.Commands.Include) ? "includeinstats" : appsettings.Discord.Commands.Include;
            StatsCommandValue.Text = appsettings.Discord.Commands.Stats.Command;
            StatsUrlValue.Text = appsettings.Discord.Commands.Stats.Url;

            //mIRCStats
            PathValue.Text = appsettings.mIRCStats.Path;
            PathBrowseButton.Click += PathBrowseButtonCommand;
            NicksFileValue.Text = string.IsNullOrEmpty(appsettings.mIRCStats.NicksFile) ? "nicks.txt" : appsettings.mIRCStats.NicksFile;
            NicksSectionValue.Text = string.IsNullOrEmpty(appsettings.mIRCStats.NickSection) ? "[common]" : appsettings.mIRCStats.NickSection;

            //Application
            FileNameValue.SelectedValue = string.IsNullOrEmpty(appsettings.Application.LoggingFileName) ? "channelid" : appsettings.Application.LoggingFileName;
            var fileNameValues = new List<ComboboxValue>();
            fileNameValues.Add(new ComboboxValue("channelid"));
            fileNameValues.Add(new ComboboxValue("channelname"));
            fileNameValues.Add(new ComboboxValue("single"));
            FileNameValues = fileNameValues;
            NotificationDelayValue.Text = appsettings.Application.NotificationDelay.ToString();
            PushoverApiKeyValue.Text = appsettings.Application.PushOver.ApiKey;
            PushoverUserKeyValue.Text = appsettings.Application.PushOver.UserKey;
            CreateNicksFileAutomaticallyValue.IsChecked = appsettings.Application.CreateNicksFileAutomatically;
            ShowDiscrimValue.IsChecked = appsettings.Application.ShowDiscrim;
            ShowAvatarValue.IsChecked = appsettings.Application.ShowAvatar;
            NicksFileManualValue.Text = appsettings.Application.NicksFileManual;
        }

        private void PathBrowseButtonCommand(object sender, RoutedEventArgs e)
        {
            var dialog = new CommonOpenFileDialog();
            dialog.IsFolderPicker = true;
            dialog.InitialDirectory = PathValue.Text;
            CommonFileDialogResult result = dialog.ShowDialog();
            if (result == CommonFileDialogResult.Ok)
                PathValue.Text = dialog.FileName;
        }

        private void LaunchGitHubSite(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start(new ProcessStartInfo
            {
                FileName = "https://github.com/MJHeijster/StatBot",
                UseShellExecute = true
            });
        }
        /// <summary>
        /// Creates the default builder.
        /// </summary>
        /// <returns>IHostBuilder.</returns>
        static IConfigurationBuilder CreateConfigurationBuilder()
        {
            return new ConfigurationBuilder()
#if DEBUG
        .AddJsonFile("appsettings.dev.json", optional: false, reloadOnChange: false);
#else
           .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false);
#endif
        }
    }
}