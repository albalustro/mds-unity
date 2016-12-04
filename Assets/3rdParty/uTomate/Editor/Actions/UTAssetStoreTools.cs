//
// Copyright (c) 2013-2016 Ancient Light Studios
// All Rights Reserved
// 
// http://www.ancientlightstudios.com
//

namespace AncientLightStudios.uTomate
{
    
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Wrapper class around the actual asset store tools. All calls are done reflectively, so this compiles
/// even when the asset store tools are not present in the project.
/// </summary>
public class UTAssetStoreTools
{
    private static bool working = false;
    private static bool lastOperationFailed = false;
    private static string lastErrorMessage = null;
    private static readonly UTAssetStorePublisher publisher = new UTAssetStorePublisher();
    private static readonly UTPackageDataSource packageDataSource = new UTPackageDataSource();

    public static bool Working
    {
        get { return working; }
    }

    public static UTAssetStorePublisher Publisher
    {
        get { return publisher; }
    }

    public static UTPackageDataSource PackageDataSource
    {
        get { return packageDataSource; }
    }

    public static bool LastOperationFailed
    {
        get { return lastOperationFailed; }
    }

    public static string LastErrorMessage
    {
        get { return lastErrorMessage; }
    }

    public static bool Available()
    {
        return UTInternalCall.GetType("AssetStoreTools.AssetStoreClient") != null;
    }

    public static void Login(string userName, string password)
    {
        StartWorking();
        UTInternalCall.InvokeStatic("AssetStoreTools.AssetStoreClient", "LoginWithCredentials", userName, password,
            false, MakeDelegate("AssetStoreTools.AssetStoreClient+DoneLoginCallback"));
    }

    public static void FetchMetadata()
    {
        StartWorking();
        UTInternalCall.InvokeStatic("AssetStoreTools.AssetStoreAPI", "GetMetaData", publisher.internalPublisher, packageDataSource.internalPackageDataSource,
            MakeDelegate("AssetStoreTools.AssetStoreAPI+DoneCallback"));
    }

    public static void ContinueWithBackgroundActions()
    {
        UTInternalCall.InvokeStatic("AssetStoreTools.AssetStoreClient", "Update");
    }

    public static bool LoggedIn()
    {
        return (bool) UTInternalCall.InvokeStatic("AssetStoreTools.AssetStoreClient", "LoggedIn");
    }

    public static bool LoginError()
    {
        return (bool) UTInternalCall.InvokeStatic("AssetStoreTools.AssetStoreClient", "LoginError");
    }

    public static void Logout()
    {
        UTInternalCall.InvokeStatic("AssetStoreTools.AssetStoreClient", "Logout");
    }

    #region Callbacks

    private static void StartWorking()
    {
        if (working)
        {
            throw new InvalidOperationException("There is still some operation in progress!");
        }

        working = true;
        lastOperationFailed = false;
        lastErrorMessage = null;
    }

    private static void OnBackgroundActionFinished(string message)
    {
        lastErrorMessage = message;
        if (!String.IsNullOrEmpty(message))
        {
            lastOperationFailed = true;
        }
        working = false;
    }

    /// <summary>
    /// Helper method creating a delegate for various callback methods of the internal API. All created delegates
    /// call <see cref="OnBackgroundActionFinished"/> when done.
    /// </summary>
    /// <param name="callbackType">the name of the type</param>
    /// <returns></returns>
    private static Delegate MakeDelegate(string callbackType)
    {
        var action = Delegate.CreateDelegate(UTInternalCall.GetType(callbackType),
            typeof(UTAssetStoreTools), "OnBackgroundActionFinished", false, true);
        return action;
    }
    #endregion

    public class UTAssetStorePublisher
    {
        public object internalPublisher;

        public UTAssetStorePublisher()
        {
            internalPublisher = UTInternalCall.CreateInstance("AssetStoreTools.AssetStorePublisher");
        }
    }


    public class UTPackageDataSource
    {
        public object internalPackageDataSource;

        public UTPackageDataSource()
        {
            internalPackageDataSource = UTInternalCall.CreateInstance("AssetStoreTools.PackageDataSource");
        }

        public List<UTPackage> GetPackageList()
        {
            var internalPackageList = new ArrayList( (ICollection) UTInternalCall.Invoke(internalPackageDataSource, "GetAllPackages"));
            var result = new List<UTPackage>(internalPackageList.Count);
            result.AddRange(from object internalPackage in internalPackageList select new UTPackage(internalPackage));
            return result;
        }

    }

    public class UTPackage
    {
        private readonly object internalPackage;

        public UTPackage(object internalPackage)
        {
            this.internalPackage = internalPackage;
        }

        public string Name
        {
            get { return (string) UTInternalCall.GetField(internalPackage, "Name"); }
        }

        public bool IsDraft
        {
            get { return ((int) UTInternalCall.Get(internalPackage, "Status")) == ((int) UTInternalCall.EnumValue("AssetStoreTools.Package+PublishedStatus", "Draft")); }
        }

        public bool IsCompleteProject
        {
            get { return (bool) UTInternalCall.GetField(internalPackage, "IsCompleteProjects"); }
        }
    }
}
}
