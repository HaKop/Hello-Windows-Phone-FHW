//
//	Dies ist die Windows Phone Demonstrations-App zu der schriftlichen Arbeit:
//	"Mobile Applikationen: Grundlagen, Entwicklung und Vermarktung"
//
//	Zweck der Applikation ist eine begleitende Demonstration fundamentaler Entwicklungs-Konzepte der Windows Phone-Entwicklung.
//	Die zugehörigen, ausführlichen Erläuterungen sind in der schriftlichen Arbeit zu finden.
//
//	Einige Konzepte, die hier demonstriert werden:
//	* Datenbindung zwischen GUI-Elementen
//  * Datenbindung zwischen GUI-Element und einem Datenmodell
//  * MVVM Entwurfsmuster
//  * Verhalten über Expression Blend einbinden
//	außerdem:
//	PanoramaViews, IsolatedStorage, TCP-Client
//
//	Kern-Funktionalität dieser App ist die Bereitstellung eines TCP-Clients, welcher sich zu einem TCP-Server
//	verbindet, welcher wiederum in der Android-App realisiert wurde. Der Musik-Service, welcher ebenfalls von 
//	der Android-App bereitgestellt wird, kann über die erfolgreich aufgebaute Socket-Verbindung gesteuert werden.
//
//	Komponenten dieses iOS-Projektes:
//	* App.xaml und Code-Behind, Einstiegs-Komponente und Event-handling
//	* MainPage.xaml und CodeBehind, View-Komponente
//  * MyServerAddress ViewModel
//  * ClientSocketClass für die Realisierung des TCP-Clients
//
//  Dieser Teil:
//  CodeBehind-Datei der App.xaml
//  Datei: App.xaml.cs
//	Version: 1.1
//
//  Created by Harald Koppay on 01.09.11.
//  Copyright 2011 Harald Koppay. All rights reserved.
//
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.IO.IsolatedStorage;

namespace HelloPhone7
{
    public partial class App : Application
    {

        /// <summary>
        /// Provides easy access to the root frame of the Phone Application.
        /// </summary>
        /// <returns>The root frame of the Phone Application.</returns>
        public PhoneApplicationFrame RootFrame { get; private set; }

        /// <summary>
        /// Constructor for the Application object.
        /// </summary>
        public App()
        {
            // Global handler for uncaught exceptions. 
            // Note that exceptions thrown by ApplicationBarItem.Click will not get caught here.
            UnhandledException += Application_UnhandledException;

            // Show graphics profiling information while debugging.
            if (System.Diagnostics.Debugger.IsAttached)
            {
                // Display the current frame rate counters.
                //Application.Current.Host.Settings.EnableFrameRateCounter = true;

                // Show the areas of the app that are being redrawn in each frame.
                //Application.Current.Host.Settings.EnableRedrawRegions = true;

                // Enable non-production analysis visualization mode, 
                // which shows areas of a page that are being GPU accelerated with a colored overlay.
                //Application.Current.Host.Settings.EnableCacheVisualization = true;
            }

            // Standard Silverlight initialization
            InitializeComponent();

            // Phone-specific initialization
            InitializePhoneApplication();
        }

        // Code to execute when the application is launching (eg, from Start)
        // This code will not execute when the application is reactivated
        private void Application_Launching(object sender, LaunchingEventArgs e)
        {
            // Wenn bereits vorhanden wird die gespeicherte Server-Adresse aus dem IsolatedStorage geladen
            // Andernfalls bleiben die Eingabefelder bzw. die Daten-Ressource für Server-IP und -Port leer
            // Die Daten-Ressource ist eine Instanz der Klasse MyServerAddress. Sie wurde in App.xaml deklariert
            if (IsolatedStorageSettings.ApplicationSettings.Contains("Ip"))
            {
                (Resources["MyServerAddressDataSource"] as MyServerAddress).Ip = IsolatedStorageSettings.ApplicationSettings["Ip"] as string;
            }
            else
            {
                (Resources["MyServerAddressDataSource"] as MyServerAddress).Ip = "";
            }

            if (IsolatedStorageSettings.ApplicationSettings.Contains("Port"))
            {
                (Resources["MyServerAddressDataSource"] as MyServerAddress).Port = IsolatedStorageSettings.ApplicationSettings["Port"] as string;
            }
            else
            {
                (Resources["MyServerAddressDataSource"] as MyServerAddress).Port = "";
            }

            // Willkommens-Nachricht in Dialog-Box anzeigen
            MessageBox.Show("Willkommen zur Windows Phone 7.1 Demo-App!\n\nDiese Anwendung ist begleitender Bestandteil der schriftlichen Arbeit 'Apps: Grundlagen, Entwicklung und Vermarktung'."+
                            "\n\nKernfunktionalität dieser App, ist die Bereitstellung einer TCP-Socket-Verbindung zu der korrespondierenden Android-Server-App.","Hello Windows Phone 7.1",MessageBoxButton.OK);
        }

        // Code to execute when the application is activated (brought to foreground)
        // This code will not execute when the application is first launched
        private void Application_Activated(object sender, ActivatedEventArgs e)
        {            

        }

        // Code to execute when the application is deactivated (sent to background)
        // This code will not execute when the application is closing
        private void Application_Deactivated(object sender, DeactivatedEventArgs e)
        {
            // Inhalte der Daten-Ressource für Server-IP und -Port werden persistent im IsolatedStorage, Datei ApplicationSettings, gespeichert
            IsolatedStorageSettings.ApplicationSettings["Ip"] = (Resources["MyServerAddressDataSource"] as MyServerAddress).Ip;
            IsolatedStorageSettings.ApplicationSettings["Port"] = (Resources["MyServerAddressDataSource"] as MyServerAddress).Port;
        }

        // Code to execute when the application is closing (eg, user hit Back)
        // This code will not execute when the application is deactivated
        private void Application_Closing(object sender, ClosingEventArgs e)
        {
            // Inhalte der Daten-Ressource für Server-IP und -Port werden persistent im IsolatedStorage, Datei ApplicationSettings, gespeichert
            IsolatedStorageSettings.ApplicationSettings["Ip"] = (Resources["MyServerAddressDataSource"] as MyServerAddress).Ip;
            IsolatedStorageSettings.ApplicationSettings["Port"] = (Resources["MyServerAddressDataSource"] as MyServerAddress).Port;
        }

        // Code to execute if a navigation fails
        private void RootFrame_NavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            if (System.Diagnostics.Debugger.IsAttached)
            {
                // A navigation has failed; break into the debugger
                System.Diagnostics.Debugger.Break();
            }
        }

        // Code to execute on Unhandled Exceptions
        private void Application_UnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
        {
            if (System.Diagnostics.Debugger.IsAttached)
            {
                // An unhandled exception has occurred; break into the debugger
                System.Diagnostics.Debugger.Break();
            }
        }

        #region Phone application initialization

        // Avoid double-initialization
        private bool phoneApplicationInitialized = false;

        // Do not add any additional code to this method
        private void InitializePhoneApplication()
        {
            if (phoneApplicationInitialized)
                return;

            // Create the frame but don't set it as RootVisual yet; this allows the splash
            // screen to remain active until the application is ready to render.
            RootFrame = new PhoneApplicationFrame();
            RootFrame.Navigated += CompleteInitializePhoneApplication;

            // Handle navigation failures
            RootFrame.NavigationFailed += RootFrame_NavigationFailed;

            // Ensure we don't initialize again
            phoneApplicationInitialized = true;
        }

        // Do not add any additional code to this method
        private void CompleteInitializePhoneApplication(object sender, NavigationEventArgs e)
        {
            // Set the root visual to allow the application to render
            if (RootVisual != RootFrame)
                RootVisual = RootFrame;

            // Remove this handler since it is no longer needed
            RootFrame.Navigated -= CompleteInitializePhoneApplication;
        }

        #endregion
    }
}