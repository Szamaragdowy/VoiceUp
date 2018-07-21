using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoiceUpServer
{
    class Protocol
    {
        //format komunikatu LOGIN/param1/param2/param3
        public string komunikat;
        //zmienna pomocnicza temp
        public bool temp = false;
        //zmienna sprawdzająca czy użytkownik jest połączony
        public bool loged = false;
        //funkcja pomocnicza by zobrazować czy parametry są poprawne czy nie
        public bool Parameters()
        {
            if (temp) return true;
            else return false;
        }
        
        public string ComunicationProtocol()
        {
            string[] datas = komunikat.Split('/');
            if (datas[0] == "CYA")
            {
                loged = false;
                return "CYA";
            }
            if (loged == true) return "AKT_USR/";//+ string z użytkownikami po /
            switch (datas[0])
            {
                case "JOIN":
                    //wyslanie klucza publicznego
                    return "SEND_P/" + "";//zamiast tego po + jakiś klucz publiczny wygenerowany
                case "LOGIN":
                    if (Parameters())
                    {
                        loged = true;
                        //sprawdzenie czy serwer nie jest pełny jesli tak
                        //return "FULL";
                        //jeśli nie
                        return "LOGIN_ACK";
                    }
                    else return "LOGIN_NAK";
                case "CHECK_Y":
                    return "AKT_USR/";//+ string z użytkownikami po /
            }
            return "Something goes wrong";//okienko messege box
        }
        public string CheckingActivity()
        {
            //co dwie minuty - nie mam pojęcia jak zrobić timer
            if (loged) return "CHECK";
            else
            {
                return "";
            }
        }
        public string CloseServer()
        {
            //do wszystkich podłączonych po TCP wysłać CYA - po otrzymaniu wszystkie powinny otrzymać komunikat że zakończono połączenie  
            return "CYA";
        }
        //trzeba by było zrobić w serwerze klasę użytkownik która by miała w sobie klase własną tego protokołu bo inaczej chyba to nie przejdzie bo każdy powinien łączyć się osobno
    }
}
