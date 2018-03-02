using Playmove.SopService;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Playmove
{
    public class PYFileManager : PYCrudBase<ArquivoVm, PYFileManager.File>
    {
        public PYFileManager() : base("Arquivos")
        {
        }

        [Serializable]
        public class File : PYDatabaseItem<ArquivoVm>
        {
            public File()
            {
                _dataVm = new ArquivoVm();
                _imageLoaded = null;
            }

            /// <summary>
            ///
            /// </summary>
            /// <param name="fullPath">Caminho completo do arquivo</param>
            /// <param name="data">Informações adicionais para esse arquivo, exemplo: Qual programa o arquivo foi feito.
            /// Essa informação é totalmente custom e cada jogo é responsavel por ela</param>
            public File(string fullPath)
                    : this(new FileInfo(fullPath))
            { }

            public File(FileInfo fileInfo)
                    : this()
            {
                Name = fileInfo.Name.Split('.')[0];
                Extension = fileInfo.Extension;
                Length = fileInfo.Length;
                FullPath = fileInfo.FullName;
            }

            public File(ArquivoVm file)
            {
                _dataVm = file ?? new ArquivoVm();
                Id = _dataVm.Id;
                Name = _dataVm.Nome;

                Localization = _dataVm.Localizacao ?? string.Empty;
                Extension = _dataVm.Extensao;
                Length = _dataVm.Tamanho;
                FullPath = _dataVm.CaminhoArquivo;

                GuidName = _dataVm.NomeFisico;
            }

            #region NotImplemented Property

            public override DateTime CreateDate
            {
                get { throw new NotImplementedException("This property should not be used in type PYFileManager.File"); }
                set { throw new NotImplementedException("This property should not be used in type PYFileManager.File"); }
            }

            public override DateTime UpdateDate
            {
                get { throw new NotImplementedException("This property should not be used in type PYFileManager.File"); }
                set { throw new NotImplementedException("This property should not be used in type PYFileManager.File"); }
            }

            public override bool Trash
            {
                get { throw new NotImplementedException("This property should not be used in type PYFileManager.File"); }
                set { throw new NotImplementedException("This property should not be used in type PYFileManager.File"); }
            }

            #endregion NotImplemented Property

            public string Localization;

            public string GuidName { get; private set; }
            public string Extension;
            public float Length;
            public string FullPath;

            [NonSerialized]
            private Sprite _imageLoaded = null;

            public override ArquivoVm ConvertTo()
            {
                _dataVm.Id = Id;
                _dataVm.Nome = Name;

                _dataVm.Localizacao = Localization ?? string.Empty;
                _dataVm.Extensao = Extension;
                _dataVm.Tamanho = Length;
                _dataVm.CaminhoArquivo = FullPath;

                return _dataVm;
            }

            /// <summary>
            /// Save on DB this instance and create the file on HD
            /// </summary>
            /// <param name="fileBytes">file bytes to write on HD</param>
            /// <param name="callback">return registry file</param>
            public void Save(byte[] fileBytes, Action<Playmove.SopRequest<Playmove.SopService.ArquivoVm>> callback)
            {
                Length = fileBytes.Length;
                PlaytableWin32.Instance.Data.FilesManager.Add(this, (request) =>
                {
                    if (request.Success)
                    {
                        FullPath = request.Model.CaminhoArquivo;
                        PYStorage.Instance.AsyncWriteFile(FullPath, fileBytes, false, (saveResults) =>
                        {
                            if (saveResults != PYStorage.SaveImageResults.SuccessfullySaved)
                            {
                                Debug.LogError("Erro ao salvar imagem");
                                PlaytableWin32.Instance.Data.FilesManager.Delete(request.Model.Id, (deleteRequest) =>
                                {
                                    if (!deleteRequest.Success)
                                        Debug.LogError("Erro ao dar rollback");
                                    else
                                    {
                                        request.Success = false;
                                        request.Message = "Erro ao salvar imagem";
                                        callback(request);
                                    }
                                });
                            }
                            else
                            {
                                callback(request);
                            }
                        });
                    }
                    else
                    {
                        callback(request);
                    }
                });
            }

            /// <summary>
            /// Make a copy of this file with a new BD registry and return it
            /// </summary>
            /// <param name="callback">null if error occurs</param>
            public void CopySaveFile(Action<ArquivoVm> callback)
            {
                if (!System.IO.File.Exists(FullPath))
                {
                    callback(null);
                    return;
                }

                PlaytableWin32.Instance.Data.FilesManager.Add(this, (result) =>
                {
                    if (result.Success)
                    {
                        PYStorage.Instance.AsyncCopyFile(this.FullPath, result.Model.CaminhoArquivo, (copyResult) =>
                        {
                            if (copyResult.HasError)
                            {
                                Debug.Log(copyResult.ErrorMessage);
                                callback(null);
                            }
                            else
                            {
                                callback(result.Model);
                            }
                        });
                    }
                    else
                    {
                        callback(null);
                    }
                });
            }

            public void LoadSpriteImg(Action<Sprite> callback, int heigh, int width)
            {
                if (_imageLoaded == null)
                    PYStorage.Instance.AsyncLoadSpriteFromPath(FullPath, heigh, width, callback);
                else
                    callback(_imageLoaded);
            }

            public void LoadSpriteImg(Action<Sprite> callback, float compress = 1)
            {
                if (_imageLoaded == null/* && Directory.Exists(FullPath)*/)
                    PYStorage.Instance.AsyncLoadSpriteFromPath(FullPath, compress, callback);
                else
                    callback(_imageLoaded);
            }

            public IEnumerator AsyncLoadSpriteImg(Action<Sprite> callback, int height = 0, int widht = 0)
            {
                if (_imageLoaded == null)
                {
                    yield return PYStorage.Instance.TrueAsyncLoadSprite(FullPath, callback, height, widht);
                }
                else
                {
                    yield return new WaitForEndOfFrame();
                    callback(_imageLoaded);
                }
            }

            public void ClearSpriteImage()
            {
                _imageLoaded = null;
                Resources.UnloadUnusedAssets();
            }

            public void SetSpriteImage(Sprite img)
            {
                _imageLoaded = img;
            }
        }

        private List<SopService.GrupoArquivosVm> _gameConfiguration = new List<SopService.GrupoArquivosVm>();

        #region Static Helper Functions

        public static void SaveFile(long gameId, long studentId, string grouping, string customData, string name, byte[] fileBytes, Action<PYStorage.SaveImageResults> callback)
        {
            File file = new File();
            file.Name = name;
            file.Extension = ".png";
            file.Length = (fileBytes.Length / 1024) / 1024;
            PYFileApplicationManager.FileApplication AppFile = new PYFileApplicationManager.FileApplication();
            AppFile.File = file;
            AppFile.AplicativoId = gameId;
            AppFile.Data = customData;
            AppFile.StudentId = studentId;
            PlaytableWin32.Instance.Data.FileApplicationManager.Add(AppFile, request =>
            {
                if (request.Success)
                    PYStorage.Instance.AsyncWriteFile(request.Model.Arquivo.CaminhoArquivo, fileBytes, false, callback);
                else
                {
                    callback(PYStorage.SaveImageResults.FailedUnknowReason);
                    Debug.LogWarning("Arquivo não foi salvo em banco. " + request.Message);
                }
            });
        }

        public static void DeleteFile(File file, Action<SopRequest<ArquivoVm>> callback = null)
        {
            PlaytableWin32.Instance.Data.FilesManager.Delete(file.Id, (result) =>
            {
                if (result.Success)
                    System.IO.File.Delete(file.FullPath);
                else
                    Debug.LogWarning("Arquivo nao deletado: " + result.Message);
                if (callback != null)
                    callback(result);
            });
        }

        #endregion Static Helper Functions

        /// <summary>
        /// Get all game files from all games installed in the Playtable
        /// </summary>
        /// <param name="callback">Callback will the files</param>
        public void GetAllGamesFiles(Action<List<File>> callback)
        {
            SopServiceFilter filter = new SopServiceFilter(SopServiceFilter.EFilters.arquivosComAluno, "false");
            GetAll(callback, filter.Parameters);
        }

        /// <summary>
        /// Get all gallery files from all games installed in the Playtable
        /// </summary>
        /// <param name="callback">Callback will the files</param>
        public void GetAllGalleriesFiles(Action<List<File>> callback)
        {
            SopServiceFilter filter = new SopServiceFilter(SopServiceFilter.EFilters.arquivosComAluno, "true");
            GetAll(callback, filter.Parameters);
        }

        /// <summary>
        /// Get all gallery files from a specific student from all games installed in the Playtable
        /// </summary>
        /// <param name="studentId">StudentId from where you want to get files</param>
        /// <param name="callback">Callback will the files</param>
        public void GetAllGalleriesFiles(long studentId, Action<List<File>> callback)
        {
            SopServiceFilter filter = new SopServiceFilter(SopServiceFilter.EFilters.alunoId, studentId.ToString());
            GetAll(callback, filter.Parameters);
        }

        /// <summary>
        /// Get all game files from a specific game
        /// </summary>
        /// <param name="jogoId">GameId from where you want to get files</param>
        /// <param name="callback">Callback will the files</param>
        public void GetGameFiles(long jogoId, Action<List<File>> callback)
        {
            SopServiceFilter filter = new SopServiceFilter(SopServiceFilter.EFilters.aplicativoId, jogoId.ToString())
                .AddFilter(SopServiceFilter.EFilters.arquivosComAluno, "false");
            GetAll(callback, filter.Parameters);
        }

        /// <summary>
        /// Get all gallery files from a specific game
        /// </summary>
        /// <param name="jogoId">GameId from where you want to get files</param>
        /// <param name="callback">Callback will the files</param>
        public void GetGameGalleryFiles(long jogoId, Action<List<File>> callback)
        {
            SopServiceFilter filter = new SopServiceFilter(SopServiceFilter.EFilters.aplicativoId, jogoId.ToString())
                .AddFilter(SopServiceFilter.EFilters.arquivosComAluno, "true");
            GetAll(callback, filter.Parameters);
        }

        /// <summary>
        /// Get all gallery files from a specific game and student
        /// </summary>
        /// <param name="jogoId">GameId from where you want to get files</param>
        /// <param name="studentId">StudentId from where you want to get files</param>
        /// <param name="callback">Callback will the files</param>
        public void GetGameGalleryFiles(long jogoId, long studentId, Action<List<File>> callback)
        {
            SopServiceFilter filter = new SopServiceFilter(SopServiceFilter.EFilters.aplicativoId, jogoId.ToString())
                .AddFilter(SopServiceFilter.EFilters.alunoId, studentId.ToString());
            GetAll(callback, filter.Parameters);
        }

        public void LoadGalleryFiles(Action<List<File>> callback)
        {
            string method = "GetArquivosGaleria";
            _service.Get<List<SopService.Dtos.ArquivoGaleriaDto>>(ACTION + method, (requests) => LoadGalleryCallback(requests, callback));
        }

        public override void Add(File data, Action<SopRequest<ArquivoVm>> callback)
        {
            AddRequest(data.ConvertTo(), callback);
        }

        public override void Delete(long id, Action<SopRequest<ArquivoVm>> callback)
        {
            DeleteRequest(id, callback);
        }

        public override void Update(File data, Action<SopRequest<ArquivoVm>> callback)
        {
            UpdateRequest(data.ConvertTo(), callback);
        }

        protected override void GetCallback(ArquivoVm request, Action<File> callback)
        {
            callback(new File(request));
        }

        protected override void GetAllCallback(List<ArquivoVm> requests, Action<List<File>> listCallback)
        {
            List<File> fs = new List<File>();
            foreach (ArquivoVm a in requests)
                fs.Add(new File(a));
            listCallback(fs);
        }

        protected void LoadGalleryCallback(List<SopService.Dtos.ArquivoGaleriaDto> requests, Action<List<File>> listCallback)
        {
            List<File> fs = new List<File>();
            Debug.LogError("Gabriel ou Jorge OLHA AQUI");
            //foreach (SopService.Dtos.ArquivoGaleriaDto a in requests)
            //    fs.Add(new File(a));
            listCallback(fs);
        }

        //protected void TranslateGalleryFilesCallback(List<SopService.Dtos.ArquivoGaleriaDto> files)
        //{
        //    List<File> fs = new List<File>();
        //    Debug.LogError("Gabriel ou Jorge OLHA AQUI");
        //    //foreach (SopService.Dtos.ArquivoGaleriaDto a in files)
        //    //    fs.Add(new File(a.Arquivo, a.Aluno, a.Turma, a.Aplicativo, a.Historico));
        //    _callbackList(fs);
        //}

        public void GetPendrivePath(long id, Action<string> callback)
        {
            SopServiceFilter filter = new SopServiceFilter(SopServiceFilter.EFilters.id, id.ToString());
            string path = CreatePath("GetPendrivePath", filter.Parameters);
            _service.Get(path, callback);
        }
    }
}