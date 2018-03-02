using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Playmove
{
    public class RegisterDropdownListClass : RegisterDropdownListGeneric
    {
        private bool _initialized;

        private List<PYClassManager.Classroom> _classes = null;

        private string NOCLASS = PYBundleManager.Localization.GetAsset<string>(PYBundleTags.Text_SemTurma, "Sem Turma");
        private RegisterDropdownItem _selected;

        #region PyOpenable

        public override void Open()
        {
            base.Open();

            Initialize();
            OpenWindow();

            ContentControl.UpPosition();

            Opened();
        }

        public override void Close()
        {
            base.Close();

            WindowAnimation.Stop();
            WindowAnimation
                .SetDuration(0.2f)
                .SetEaseType(Ease.Type.InBack);
            WindowAnimation.Reverse(() => Closed());
        }

        #endregion PyOpenable

        /// TODO: Instanciar elementos em tempo de execução
        /// TODO: Melhorar Paginação de para filtros
        public override void Initialize()
        {
            if (_initialized)
                return;
            _initialized = true;

            _itensHolderPosition = ItensHolder.transform.localPosition;
            BoxCollider2D coll = gameObject.AddComponent<BoxCollider2D>();
            coll.size = _maskSize;
            coll.offset = MaskTransform.localPosition;
            ContentDrag.Content = ItensHolder;

            if (_classes == null)
                PlaytableWin32.Instance.Data.ClassManager.GetAll(GetAllClassroomsCallback);
            else
                InstantiateItens();
        }

        private void GetAllClassroomsCallback(List<PYClassManager.Classroom> classrooms)
        {
            _classes = new List<PYClassManager.Classroom>();
            _classes.Add(new PYClassManager.Classroom(NOCLASS, 0, 0));
            _classes.AddRange(classrooms);
            InstantiateItens();
        }

        private void InstantiateItens()
        {
            if (_createdItens.Count == 0)
            {
                for (int i = 0; i < _classes.Count; i++)
                {
                    RegisterDropdownItem newItem = Instantiate(ListItemPrefab, ItensHolder.position + (Vector3.down * i * _elementsDistance), Quaternion.identity) as RegisterDropdownItem;
                    newItem.transform.SetParent(ItensHolder);
                    newItem.MyButton.onClick.AddListener(ItemClicked);
                    newItem.ListIndex = _createdItens.Count;
                    newItem.Mask = _mask;
                    _createdItens.Add(newItem);
                }
            }
            else
            {
                foreach (RegisterDropdownItem name in _createdItens)
                {
                    name.RemoveItem();
                }
            }

            SetItensText(_classes);
        }

        public int GetSelectedClassIndex()
        {
            return _createdItens.Where((item) => item.Text == LabelText).Select(item => item.ListIndex).SingleOrDefault();
        }

        public long GetSelectedClassId()
        {
            //return _selected == null ? 0 : _selected.RegisterId;
            return _classes.Where(x => x.Name == LabelText).Select(x => x.Id).SingleOrDefault();
        }

        public void SetClass(int id)
        {
            _selected = _createdItens.FirstOrDefault(x => x.RegisterId == id);
            GetComponentInParent<RegisterDropdownManager>().LabelText = _selected != null ? _selected.Text : NOCLASS;
        }

        private void ItemClicked(PYButton newItem)
        {
            onItemClicked.Invoke(newItem.transform.parent.GetComponent<RegisterDropdownItem>());
        }

        private void SetItensText(List<PYClassManager.Classroom> itens)
        {
            for (int i = 0; i < _createdItens.Count; i++)
            {
                if (i < itens.Count)
                {
                    _createdItens[i].SetItem(itens[i].Name, itens[i].Id);
                }
                else
                {
                    _createdItens[i].RemoveItem();
                }
            }
            CalculateDragLimit();
        }
    }
}