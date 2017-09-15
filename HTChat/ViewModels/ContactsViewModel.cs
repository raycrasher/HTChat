using System.Collections.ObjectModel;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using HTChat.Models;
using System.ComponentModel;
using System.Collections.Generic;
using System.Windows.Input;
using System;
using agsXMPP;
using agsXMPP.protocol.client;
using GalaSoft.MvvmLight.Threading;

namespace HTChat.ViewModels
{
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// Use the <strong>mvvminpc</strong> snippet to add bindable properties to this ViewModel.
    /// </para>
    /// <para>
    /// You can also use Blend to data bind with the tool's support.
    /// </para>
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class ContactsViewModel : ViewModelBase, INotifyPropertyChanged
    {
        private ChatClient _client;

        public string Title => "HTChat";

        public ViewModelBase CurrentView { get; set; }
        public ObservableCollection<Contact> Contacts { get; private set; }
        private Dictionary<Jid, Contact> _jidContactMap { get; set; }

        

        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public ContactsViewModel()
        {
            if (IsInDesignMode)
            {
                Contacts = new ObservableCollection<Contact>(new[] {
                    new Contact{ Jid = "test@test.com", Status= "Online"},
                    new Contact{ Jid = "jude@test.com", Status= "Away"},
                    new Contact{ Jid = "rey@test.com", Status= "Offline very long status. ABCDEFGHIJKLMNOPQRSTUVWXYZ"},
                    new Contact{ Jid = "shirlyn@test.com", Status= "Idle"},
                    new Contact{ Jid = "jellie@test.com", Status= "Online"}
                });
            }
            else
            {
                _client = SimpleIoc.Default.GetInstance<ChatClient>();
                Contacts = new ObservableCollection<Contact>();
                _jidContactMap = new Dictionary<Jid, Contact>();

                _client.XmppClient.OnPresence += OnXmppPresence;
            }
        }

        private void OnXmppPresence(object sender, Presence pres)
        {
            DispatcherHelper.CheckBeginInvokeOnUI(() => {
                if(_jidContactMap.TryGetValue(pres.From, out var contact))
                {
                    contact.Status = pres.Status;
                }
                else
                {
                    contact = new Contact { Jid = pres.From, Status = pres.Status };
                    _jidContactMap[pres.From] = contact;
                    Contacts.Add(contact);
                }
            });
        }

        public ICommand StartSingleChatCmd => new DelegateCommand(o => StartSingleChat((Contact)o));

        private void StartSingleChat(Contact contact)
        {
            MainViewModel.Instance.ActiveChatView = new SingleChatViewModel(contact);
        }
    }
}