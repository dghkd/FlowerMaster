using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Windows.Media;
using System.Diagnostics;

namespace FKGDataEditor
{
    public class GirlInfoVM : ViewModelBase
    {
        #region Private Member
        private GirlInfo _info;
        #endregion


        #region Constructor
        public GirlInfoVM(GirlInfo info)
        {
            _info = info;
        }
        #endregion


        #region Public Member
        public int ID
        {
            get { return _info.ID; }
            set { _info.ID = value; OnPropertyChanged("ID"); }
        }

        public ImageSource ImageSrc
        {
            get
            {
                if (_info.ImageSrc == null)
                {
                    _info.ImageSrc = GirlInfo.Base642Image(this.ImgBase64);
                }
                return _info.ImageSrc;
            }
            set { _info.ImageSrc = value; OnPropertyChanged("ImageSrc"); }
        }

        public String ImgBase64
        {
            get { return _info.ImgBase64; }
            set
            {
                _info.ImgBase64 = value;
                _info.ImageSrc = GirlInfo.Base642Image(this.ImgBase64);
                OnPropertyChanged("ImgBase64"); OnPropertyChanged("ImageSrc");
            }
        }

        public String Names
        {
            get
            {
                if (_info.Names == ""
                    || _info.Names == null)
                {
                    return _info.NamesJPN;
                }
                return _info.Names;
            }
            set { _info.Names = value; OnPropertyChanged("Names"); }
        }

        public String NamesJPN
        {
            get { return _info.NamesJPN; }
            set { _info.NamesJPN = _info.Names = value; OnPropertyChanged("NamesJPN"); }
        }

        public String NamesCHT
        {
            get { return _info.NamesCHT; }
            set { _info.NamesCHT = value; OnPropertyChanged("NamesCHT"); }
        }
        public String NamesCHS
        {
            get { return _info.NamesCHS; }
            set { _info.NamesCHS = value; OnPropertyChanged("NamesCHS"); }
        }
        public String NamesENU
        {
            get { return _info.NamesENU; }
            set { _info.NamesENU = value; OnPropertyChanged("NamesENU"); }
        }
        public int FKGID
        {
            get { return _info.FKGID; }
            set { _info.FKGID = value; OnPropertyChanged("FKGID"); }
        }

        public int Rare
        {
            get { return _info.Rare; }
            set { _info.Rare = value; OnPropertyChanged("Rare"); }
        }
        public GirlInfoEnum.Types Type
        {
            get { return _info.Type; }
            set { _info.Type = value; OnPropertyChanged("Type"); }
        }
        public GirlInfoEnum.Nationalities Nationality
        {
            get { return _info.Nationality; }
            set { _info.Nationality = value; OnPropertyChanged("Nationality"); }
        }

        public String Note
        {
            get { return _info.Note; }
            set { _info.Note = value; OnPropertyChanged("Note"); }
        }

        public String WikiUrlJPN
        {
            get
            {
                String wikiUrl = @"http://フラワーナイトガール.攻略wiki.com/index.php?";
                String name = this.NamesJPN;

                wikiUrl += name;

                return wikiUrl;
            }
        }

        public String WikiUrlENU
        {
            get
            {
                String wikiUrl = @"http://flowerknight.wikia.com/wiki/";
                String name = this.NamesENU.Replace(" ", "_");
                wikiUrl += name;

                return wikiUrl;
            }
        }

        public override string ToString()
        {
            return String.Format("{0} {1} {2}", this.NamesJPN, this.NamesENU, this.NamesCHT);

        }
        #endregion


        #region Command

        public const string CmdKey_OpenWikiUrl_JPN = "CmdKey_OpenWikiUrl_JPN";
        public const string CmdKey_OpenWikiUrl_ENU = "CmdKey_OpenWikiUrl_ENU";

        private CommandBase _cmdOpenWikiUrl_JPN;
        private CommandBase _cmdOpenWikiUrl_ENU;

        public CommandBase CmdOpenWikiUrl_JPN
        {
            get
            {
                return _cmdOpenWikiUrl_JPN ?? (_cmdOpenWikiUrl_JPN = new CommandBase(x => ExecuteCommand(CmdKey_OpenWikiUrl_JPN)));
            }
        }
        public CommandBase CmdOpenWikiUrl_ENU
        {
            get
            {
                return _cmdOpenWikiUrl_ENU ?? (_cmdOpenWikiUrl_ENU = new CommandBase(x => ExecuteCommand(CmdKey_OpenWikiUrl_ENU)));
            }
        }

        public Func<String, GirlInfoVM, bool> CommandAction { get; set; }

        private void ExecuteCommand(String cmdKey)
        {
            switch (cmdKey)
            {
                case CmdKey_OpenWikiUrl_JPN:
                    Process.Start(this.WikiUrlJPN);
                    break;

                case CmdKey_OpenWikiUrl_ENU:
                    Process.Start(this.WikiUrlENU);
                    break;

                default:
                    break;
            }

            if (null != this.CommandAction)
            {
                CommandAction(cmdKey, this);
            }
        }

        #endregion


        #region Public Method
        public GirlInfo GetDataInfo()
        {
            return _info;
        }
        #endregion
    }
}
