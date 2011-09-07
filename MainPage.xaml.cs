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
//	* MainPage.xaml und CodeBehind
//  * MyServerAddress ViewModel
//  * ClientSocketClass für die Realisierung des TCP-Clients
//
//  Dieser Teil:
//  CodeBehind-Datei der MainPage.xaml
//  Datei: MainPage.xaml.cs
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
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Shell;
using System.IO.IsolatedStorage;
using Microsoft.Phone.Net.NetworkInformation;

namespace HelloPhone7
{
    public partial class MainPage : PhoneApplicationPage
    {

        ClientSocketClass client = null;

        // Constructor
        public MainPage()
        {
            InitializeComponent();

            // Test auf Netzwerkverbindung und Anpassung der GUI-Elemente je nach Netz-Status
            if (!DeviceNetworkInformation.IsNetworkAvailable)
            {
                // Steuerungselemente (play, ff, stop buttons) des TCP-Clients werden deaktiviert wenn kein Netzwerk vorhanden ist
                ((IApplicationBarIconButton)ApplicationBar.Buttons[0]).IsEnabled = false;
                ((IApplicationBarIconButton)ApplicationBar.Buttons[1]).IsEnabled = false;
                ((IApplicationBarIconButton)ApplicationBar.Buttons[2]).IsEnabled = false;
                textfield_server.Text = "network not available...";
                textfield_output.Text = "Keine aktive Netzwerkverbindung...";
            }
        }

        /// <summary>
        /// Event-Handler-Methode zur Verarbeitung des Clicks auf den Play-Button der TCP-Client-Steuerungsleiste
        /// Es wird zum angegebenen Host verbunden und das "MUSIC PLAY"-Kommando abgeschickt.
        /// </summary>
        /// <param name="sender">geklicktes GUI-Element, in diesem Fall immer der Play-Button</param>
        private void bar_btn_play_Click(object sender, EventArgs e)
        {

            // Ausschluss einer leeren Host-Adresse
            if (ValidateRemoteHost())
            {
                // Instanziieren der ClientSocket-Klasse
                client = new ClientSocketClass();

                // Verbindung zum TCP-Server aufbauen...
                string result = client.Connect(edit_IP.Text, int.Parse(edit_Port.Text));
                // ...und absenden des "MUSIC PLAY"-Kommandos
                result = client.Send("MUSIC PLAY\n");
                // result auf Erfogsergebnis prüfen
                if (result.Equals("Success"))
                {
                    // Antwort des Servers entgegennehmen
                    result = client.Receive();
                    // Antwort ggf. für die Ausgabe kürzen
                    result = result.Replace("Playback stopped!", "");
                    // Bei leerem Antwortstring, nächsten Antwortstring des Servers entgegennehmen
                    if (result.Equals(""))
                    {
                        result = client.Receive();
                    }
                    // Antwortstrings, die mit "Playing:" beginnen, direkt im TextView ausgeben
                    if (result.Contains("Playing:"))
                    {
                        textfield_server.Text = "Connected to " + edit_IP.Text + "...";
                        textfield_output.Text = ">> "+result;
                        // Verbindung zum Server und Service besteht. TCP-Client-Steuerungselemente anpassen
                        ((IApplicationBarIconButton)ApplicationBar.Buttons[1]).IsEnabled = true;
                        ((IApplicationBarIconButton)ApplicationBar.Buttons[2]).IsEnabled = true;
                        ((IApplicationBarIconButton)ApplicationBar.Buttons[0]).IsEnabled = false;
                    }
                // result enthält Fehlermeldung, Verbindung wurde nicht hergestellt
                } else
                {
                    // disconnect()-Routine aufrufen
                    disconnect();
                    // Entsprechende Meldungen ausgeben
                    textfield_server.Text = "connection failed...";
                    textfield_output.Text = "Prüfen Sie die eingegebene Zieladresse...";
                }
            }
        }

        /// <summary>
        /// Prüfung des Eingabefeldes für die Server-Adresse auf eine Eingabe
        /// </summary>
        /// <returns>TRUE, wenn die Adress-Felder nicht leer sind</returns>
        private bool ValidateRemoteHost()
        {
            if (String.IsNullOrWhiteSpace(edit_IP.Text) || String.IsNullOrWhiteSpace(edit_Port.Text))
            {
                MessageBox.Show("Server-IP und Port angeben!");
                return false;
            }

            return true;
        }

        /// <summary>
        /// Event-Handler-Methode zur Verarbeitung des Clicks auf den FF-Button der TCP-Client-Steuerungsleiste
        /// Es wird das "MUSIC PLAY"-Kommando an den TCP-Server geschickt, welches das nächste Musikstück abspielt.
        /// </summary>
        /// <param name="sender">geklicktes GUI-Element, in diesem Fall immer der FF-Button</param>
        private void bar_btn_ff_Click(object sender, EventArgs e)
        {
            // Absenden des "MUSIC PLAY"-Kommandos
            String result = client.Send("MUSIC PLAY\n");
            // Erfolgsmeldung zurückgegeben
            if (result.Equals("Success"))
            {
                // Antwort des Servers entgegennehmen
                result = client.Receive();
                // Antwort ggf. für die Ausgabe kürzen
                result = result.Replace("Playback stopped!", "");
                // Bei leerem Antwortstring, nächsten Antwortstring des Servers entgegennehmen
                if (result.Equals(""))
                {
                    result = client.Receive();
                }
                if (result.Contains("Playing:"))
                {
                    // Antwortstrings, die mit "Playing:" beginnen, direkt im TextView ausgeben
                    textfield_output.Text = ">> " + result;
                }
            // Fehlermeldung zurückgegeben, Verbindung zum Server unterbrochen
            }else{
                // disconnect()-Routine aufrufen
                disconnect();
                // Entsprechende Meldungen ausgeben
                textfield_server.Text = "operation timed out...";
                textfield_output.Text = "Verbindung zum Server wurde unterbrochen...";
            }
        }

        /// <summary>
        /// Event-Handler-Methode zur Verarbeitung des Clicks auf den STOP-Button der TCP-Client-Steuerungsleiste
        /// Es wird das "MUSIC STOP"-Kommando an den TCP-Server geschickt und die Verbindung beendet.
        /// </summary>
        /// <param name="sender">geklicktes GUI-Element, in diesem Fall immer der STOP-Button</param>
        private void bar_btn_stop_Click(object sender, EventArgs e)
        {
            // Stoppen der Musik-Wiedergabe auf dem Server
            client.Send("MUSIC STOP\n");
            // disconnect()-Routine aufrufen
            disconnect();
            // Entsprechende Meldungen ausgeben
            textfield_server.Text = "not connected...";
            textfield_output.Text = "'play' drücken um Verbindung aufzubauen...";
        }

        /// <summary>
        /// Hilfsmethode zum Beenden der Socket-Verbindung und Anpassung der TCP-Client-Steuerungselemente
        /// </summary>
        private void disconnect()
        {
            // Verbindung schliessen
            client.Close();
            ((IApplicationBarIconButton)ApplicationBar.Buttons[1]).IsEnabled = false;
            ((IApplicationBarIconButton)ApplicationBar.Buttons[2]).IsEnabled = false;
            ((IApplicationBarIconButton)ApplicationBar.Buttons[0]).IsEnabled = true; ;
        }

    }
}