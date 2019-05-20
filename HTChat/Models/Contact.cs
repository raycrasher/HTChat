using agsXMPP;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using agsXMPP.protocol.iq.vcard;
using System.Windows.Media.Imaging;

namespace HTChat.Models
{
    public class Contact: INotifyPropertyChanged
    {
        public Contact() { }

        public Contact(Jid jid)
        {
            Jid = jid;
        }

        public Jid Jid { get; set; }
        public ImageSource Avatar { get; set; } = DefaultPhoto;
        public string DisplayName => string.IsNullOrEmpty(Vcard?.Fullname) ? Jid.UnescapeNode(Jid.Bare) : Vcard.Fullname;
        public Vcard Vcard { get; set; }
        public string Status { get; set; }
        public string Nickname
        {
            get
            {
                if (string.IsNullOrWhiteSpace(Vcard?.Nickname) && !string.IsNullOrWhiteSpace(Vcard?.Fullname))
                    return Vcard.Fullname.Substring(0, Vcard.Fullname.IndexOf(" "));
                else return DisplayName;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public static ImageSource DefaultPhoto = new BitmapImage(new System.Uri("pack://application:,,,/HTChat;component/Resources/user-icon-placeholder.png"));
    }
}
