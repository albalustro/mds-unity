using Playmove;
using Playtable.Models.Playtable;
using System;
using System.Collections.Generic;

public class PYPlaytableInfoManager : PYCrudBase<PlaytableInfoVm, PYPlaytableInfoManager.PlaytableInfo>
{
    public class PlaytableInfo
    {
        public PlaytableInfo()
        {
        }

        public PlaytableInfo(PlaytableInfoVm data)
        {
            _data = data;
            if (_data == null) return;
            Id = _data.Id;
            Name = _data.Nome;
            Email = _data.Email;
            PhoneNumber = _data.Telefone;
            City = _data.Cidade;
            State = _data.Estado;
        }

        public long Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string City { get; set; }
        public string State { get; set; }

        private PlaytableInfoVm _data;

        public PlaytableInfoVm ConvertTo()
        {
            if (_data == null)
                _data = new PlaytableInfoVm();
            _data.Id = Id;
            _data.Nome = Name;
            _data.Email = Email;
            _data.Telefone = PhoneNumber;
            _data.Cidade = City;
            _data.Estado = State;

            return _data;
        }
    }

    public PYPlaytableInfoManager() : base("PlaytableInfos")
    {
    }

    public override void Add(PlaytableInfo data, Action<SopRequest<PlaytableInfoVm>> callback)
    {
        AddRequest(data.ConvertTo(), callback);
    }

    public override void Delete(long id, Action<SopRequest<PlaytableInfoVm>> callback)
    {
        DeleteRequest(id, callback);
    }

    public override void Update(PlaytableInfo data, Action<SopRequest<PlaytableInfoVm>> callback)
    {
        UpdateRequest(data.ConvertTo(), callback);
    }

    protected override void GetCallback(PlaytableInfoVm request, Action<PlaytableInfo> callback)
    {
        callback(new PlaytableInfo(request));
    }

    protected override void GetAllCallback(List<PlaytableInfoVm> requests, Action<List<PlaytableInfo>> listCallback)
    {
        List<PlaytableInfo> infos = new List<PlaytableInfo>();
        foreach (PlaytableInfoVm vm in requests)
        {
            infos.Add(new PlaytableInfo(vm));
        }
        listCallback(infos);
    }
}