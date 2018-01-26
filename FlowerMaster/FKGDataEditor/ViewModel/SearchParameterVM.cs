using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FKGDataEditor
{
    public class SearchParameterVM : ViewModelBase
    {
        #region Private Member
        private String _keyword;
        private GirlInfoEnum.Types _type;
        private GirlInfoEnum.Nationalities _nationality;
        private int _rare;
        #endregion


        #region Constructor
        public SearchParameterVM()
        {
            _keyword = "";
            _type = GirlInfoEnum.Types.NotCare;
            _nationality = GirlInfoEnum.Nationalities.NotCare;
            _rare = 0;
        }
        #endregion


        #region Public Member
        public String Keyword
        {
            get { return _keyword; }
            set { _keyword = value; OnPropertyChanged("Keyword"); }
        }

        public GirlInfoEnum.Types Type
        {
            get { return _type; }
            set { _type = value; OnPropertyChanged("Type"); }
        }

        public GirlInfoEnum.Nationalities Nationality
        {
            get { return _nationality; }
            set { _nationality = value; OnPropertyChanged("Nationality"); }
        }

        public int Rare
        {
            get { return _rare; }
            set { _rare = value; OnPropertyChanged("Rare"); }
        }

        public override string ToString()
        {
            String ret = String.Format("Keyword={0} Type={1} Nationality={2} Rare={3}", this.Keyword, this.Type, this.Nationality, this.Rare);
            return ret;
        }
        #endregion
    }
}
