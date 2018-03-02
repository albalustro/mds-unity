using System;
using System.Collections.Generic;
using UnityEngine;

namespace Playmove
{
    public abstract class PYCrudBase<SopType, APIType>
        where SopType : new()
        where APIType : new()
    {
        protected string ACTION;

        protected SopServiceMethods _service = new SopServiceMethods();

        public PYCrudBase(string action)
        {
            ACTION = action + "/";
        }

        public void Get(Action<APIType> callback)
        {
            Get(callback, new Dictionary<string, string>());
        }

        public virtual void Get(long id, Action<APIType> callback)
        {
            SopServiceFilter filter = new SopServiceFilter(SopServiceFilter.EFilters.id, id.ToString());
            Get(callback, filter.Parameters);
        }

        public virtual void Get(Action<APIType> callback, Dictionary<string, string> filters)
        {
            string path = CreatePath("Get", filters);
            _service.Get<SopType>(path, (request) => GetCallback(request, callback));
        }

        public virtual void GetAll(Action<List<APIType>> callback)
        {
            _service.Get<List<SopType>>(ACTION + "GetAll", (requests) => GetAllCallback(requests, callback));
        }

        public virtual void GetAll(bool inTrash, Action<List<APIType>> callback)
        {
            SopServiceFilter filter = new SopServiceFilter(SopServiceFilter.EFilters.lixeira, inTrash.ToString());
            GetAll(callback, filter.Parameters);
        }

        public virtual void GetAll(Action<List<APIType>> callback, Dictionary<string, string> filters)
        {
            string path = CreatePath("GetAll", filters);
            _service.Get<List<SopType>>(path, (requests) => GetAllCallback(requests, callback));
        }

        protected virtual void AddRequest(SopType data, Action<SopRequest<SopType>> callback)
        {
            _service.Post<SopType>(ACTION + "Add", data, callback);
        }

        protected virtual void DeleteRequest(long id, Action<SopRequest<SopType>> callback)
        {
            //_service.Post<SopType>(ACTION + "Delete", data, callback);
            //SopServiceFilter filter = new SopServiceFilter(SopServiceFilter.EFilters.id, id.ToString());
            //string path = CreatePath("Delete", filter.Parameters);
            _service.Post(ACTION + "Delete", id, callback);
        }

        protected virtual void UpdateRequest(SopType data, Action<SopRequest<SopType>> callback)
        {
            _service.Post<SopType>(ACTION + "Update", data, callback);
        }

        public abstract void Add(APIType data, Action<SopRequest<SopType>> callback);

        public abstract void Delete(long id, Action<SopRequest<SopType>> callback);

        public abstract void Update(APIType data, Action<SopRequest<SopType>> callback);

        public string CreatePath(string method, Dictionary<string, string> filters)
        {
            string path = string.Format("{0}{1}?", ACTION, method);

            foreach (string key in filters.Keys)
            {
                path += string.Format("{0}={1}&", key, filters[key]);
            }

            path = path.Remove(path.Length - 1);
            return WWW.EscapeURL(path);
        }

        protected abstract void GetCallback(SopType request, Action<APIType> callback);

        protected abstract void GetAllCallback(List<SopType> requests, Action<List<APIType>> listCallback);
    }
}