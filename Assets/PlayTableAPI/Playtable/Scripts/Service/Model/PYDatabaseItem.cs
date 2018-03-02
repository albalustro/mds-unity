using System;
using UnityEngine;

namespace Playmove
{
    public interface IPYDatabaseItem
    {
        long Id { get; set; }
        string Name { get; set; }
        DateTime CreateDate { get; set; }
        DateTime UpdateDate { get; set; }
        bool Trash { get; set; }
    }

    [Serializable]
    public abstract class PYDatabaseItem<T> : IPYDatabaseItem
    {
        [SerializeField]
        private long _id;
        public long Id
        {
            get { return _id; }
            set { _id = value; }
        }

        [SerializeField]
        private string _name;
        public virtual string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        [SerializeField]
        private DateTime _createDate;
        public virtual DateTime CreateDate
        {
            get { return _createDate; }
            set { _createDate = value; }
        }

        [SerializeField]
        private DateTime _updateDate;
        public virtual DateTime UpdateDate
        {
            get { return _updateDate; }
            set { _updateDate = value; }
        }

        [SerializeField]
        private bool _trash;
        public virtual bool Trash
        {
            get { return _trash; }
            set { _trash = value; }
        }
        
        [NonSerialized]
        protected T _dataVm;

        public abstract T ConvertTo();
    }
}
