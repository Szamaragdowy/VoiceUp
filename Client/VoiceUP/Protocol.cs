using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoiceUP
{
    class Protocol
    {
        //format komunikatu LOGIN/param1/param2/param3
        public string komunikat="";
        public string FirstLogToServer()
        {
            string[] datas = komunikat.Split('/');
            switch (datas[0])
            {
                case "":
                    return "JOIN";
                case "SEND_P":
                    //wpisywanie danych zaszyfrowanych otrzymanym kluczem publicznym w datas[1]
                    return "LOGIN"; // dodatkowo dopisać po "/" login haslo suma kontrolna
            }
            return CommunicationProtocol();
        }
        public string CommunicationProtocol()
        {
            string[] kek = komunikat.Split('/');
            switch (kek[0])
            {
                case "LOGIN_ACK":
                    break;
                case "LOGIN_NAK":
                    //Okienko message box o tym że nie można ustanowić połączenia z powodu błędnych parametrów
                    break;
                case "CHECK":
                    return "CHECK_Y";
                case "AKT_USR":
                    //W pozostałych parametrach są użytkownicy na serwerze więc wyświetlanie ich będzie pokazane w gridzie
                    break;
                case "FULL":
                    //Okienko message box o tym że serwer jest pełny i nie można ustanowić połączenia oraz powracamy do grida w którym łączymy się z serwerem
                    break;
                default:
                    break;
            }
            return "Something goes wrong";//okienko messege box
        }
        public string CloseComunication()
        {
            //doputy serwer nie odpowie nam cya nie zamykamy aplikacji
            if (komunikat == "CYA") return "";//wykonaj zakończenie komunikacji/zamknięcie aplikacji/opuściłeś server albo server został wyłączony.
            else return "CYA";
        }
    }
}
