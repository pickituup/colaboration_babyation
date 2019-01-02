// Helpers/Settings.cs
using Plugin.Settings;
using Plugin.Settings.Abstractions;
using System;

namespace BabyationApp.Helpers
{
	/// <summary>
	/// This is the Settings static class that can be used in your Core solution or in any
	/// of your client applications. All settings are laid out the same exact way with getters
	/// and setters. 
	/// </summary>
	public static class Settings
	{
		private static ISettings AppSettings
		{
			get
			{
				return CrossSettings.Current;
			}
		}

		#region Setting Constants

		private const string SettingsKey = "settings_key";
		private static readonly string SettingsDefault = "yes";
        

		#endregion


        public static string KnownExperiences
        {
            get
            {
                return AppSettings.GetValueOrDefault("KnownExperiences", string.Empty);
            }
            set
            {
                AppSettings.AddOrUpdateValue("KnownExperiences", value);
            }
        }

		public static string IsFirstTimeUser
		{
			get
			{
				return AppSettings.GetValueOrDefault(SettingsKey, SettingsDefault);
			}
			set
			{
				AppSettings.AddOrUpdateValue(SettingsKey, value);
			}
		}

        public static string Token
        {
            get
            {
                return AppSettings.GetValueOrDefault("Token", string.Empty);
            }
            set
            {
                AppSettings.AddOrUpdateValue("Token", value);
            }
        }

        public static string UserId
        {
            get
            {
                return AppSettings.GetValueOrDefault("UserId", string.Empty);
            }
            set
            {
                AppSettings.AddOrUpdateValue("UserId", value);
            }
        }

        public static string Expires
        {
            get
            {
                return AppSettings.GetValueOrDefault("Expires", DateTime.MinValue.ToString());
            }
            set
            {
                AppSettings.AddOrUpdateValue("Expires", value);
            }
        }

        public static string UserName
        {
            get
            {
                return AppSettings.GetValueOrDefault("UserName", string.Empty);
            }
            set
            {
                AppSettings.AddOrUpdateValue("UserName", value);
            }
        }

        public static string Name
        {
            get
            {
                return AppSettings.GetValueOrDefault("Name", string.Empty);
            }
            set
            {
                AppSettings.AddOrUpdateValue("Name", value);
            }
        }


        public static string UserEmail
        {
            get
            {
                return AppSettings.GetValueOrDefault("UserEmail", string.Empty);
            }
            set
            {
                AppSettings.AddOrUpdateValue("UserEmail", value);
            }
        }

        public static string SessionStart
        {
            get
            {
                return AppSettings.GetValueOrDefault("SessionStart", string.Empty);
            }
            set
            {
                AppSettings.AddOrUpdateValue("SessionStart", value);
            }
        }

        public static int Provider
        {
            get
            {
                return AppSettings.GetValueOrDefault("Provider", -1);
            }
            set
            {
                AppSettings.AddOrUpdateValue("Provider", value);
            }
        }

    public static long SessionUptime
        {
            get
            {
                return AppSettings.GetValueOrDefault("SessionUptime", -1L);
            }
            set
            {
                AppSettings.AddOrUpdateValue("SessionUptime", value);
            }
        }

        public static string CaregiverCode
        {
            get => AppSettings.GetValueOrDefault("CaregiverCode", null);
            set => AppSettings.AddOrUpdateValue("CaregiverCode", value);
        }
    }
}