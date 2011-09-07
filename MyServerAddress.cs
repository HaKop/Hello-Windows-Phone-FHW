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
//  MyServerAddress ViewModel
//  Datei: MyServerAddress.cs
//	Version: 1.1
//
//  Created by Harald Koppay on 01.09.11.
//  Copyright 2011 Harald Koppay. All rights reserved.
//
using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.ComponentModel;

namespace HelloPhone7
{
    // Klasse implementiert das INotifyPropertyChanged-Interface und kann somit gebundene GUI-Elemente über Änderungen der Attribute informieren
    public class MyServerAddress : INotifyPropertyChanged
    {
        // IP-Attribut
        private string _ip = "";
        public string Ip
        {
            // Getter-Methode
            get { return _ip; }
            // Setter-Methode ruft die Methode NotifyPropertyChanged sofern sich der Attribut-Wert ändert
            set
            {
                if (!_ip.Equals(value)) { 
					_ip = value;
					NotifyPropertyChanged("Ip"); 
				}
            }
        }

        // Port-Attribut
        private string _port = "";
        public string Port
        {
            // Getter-Methode
            get { return _port; }
            // Setter-Methode ruft die Methode NotifyPropertyChanged sofern sich der Attribut-Wert ändert
            set
            {
                if (!_port.Equals(value)) { 
					_port = value;
					NotifyPropertyChanged("Port"); 
				}
            }
        }

        /// <summary>
        /// NotifyPropertyChanged informiert jedes gebundene GUI-Element über etwaige Änderungen der in dieser Klasse enthaltenen Attribute
        /// </summary>
        /// <param name="p">Name des Attributes, dessen Wert sich geändert hat</param>
        private void NotifyPropertyChanged(string p)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(p));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
