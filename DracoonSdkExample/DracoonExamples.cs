﻿using Dracoon.Sdk.Error;
using Dracoon.Sdk.Filter;
using Dracoon.Sdk.Model;
using Dracoon.Sdk.Sort;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;

namespace Dracoon.Sdk.Example {
    public static class DracoonExamples {

        private static readonly Uri SERVER_URI = new Uri("https://dracoon.team");
        private static readonly string ACCESS_TOKEN = "access-token";
        private static readonly string ENCRYPTION_PASSWORD = "encryption-password";

        private static DracoonClient dc;

        [STAThread]
        static void Main() {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;

            DracoonAuth dracoonAuth = new DracoonAuth(ACCESS_TOKEN);
            IWebProxy wp = WebRequest.GetSystemWebProxy();
            wp.Credentials = CredentialCache.DefaultNetworkCredentials;
            DracoonHttpConfig config = new DracoonHttpConfig(retryEnabled: true, webProxy: wp);
            dc = new DracoonClient(SERVER_URI, dracoonAuth, ENCRYPTION_PASSWORD, new Logger(), config);
        }

        #region DracoonClient.Server

        private static void GetServerData() {
            string serverVersion = dc.Server.GetVersion();
            Console.WriteLine("Server version: " + serverVersion);

            DateTime? serverTime = dc.Server.GetTime();
            if (serverTime.HasValue) {
                Console.WriteLine("Server time: " + serverTime.Value.ToLocalTime());
            }
        }

        private static void GetServerSettings() {
            ServerGeneralSettings generalSettings = dc.Server.ServerSettings.GetGeneral();
            Console.WriteLine("Crypto is enabled: " + generalSettings.CryptoEnabled);
            Console.WriteLine("Media server is enabled: " + generalSettings.MediaServerEnabled);
            Console.WriteLine("Share password via SMS is enabled: " + generalSettings.SharePasswordSmsEnabled);
            Console.WriteLine("Weak passwords are enabled: " + generalSettings.WeakPasswordEnabled);

            ServerInfrastructureSettings infrastructureSettings = dc.Server.ServerSettings.GetInfrastructure();
            //...
        }

        #endregion

        #region DracoonClient.Account

        private static void CheckAuth() {
            try {
                dc.Account.ValidateTokenValidity();
                Console.WriteLine("Tokens are still valid.");
            } catch (DracoonApiException apiError) {
                if (apiError.ErrorCode.IsAuthError()) {
                    Console.WriteLine("Tokens are not valid anymore.");
                }
            }
        }

        private static void GetUserAccount() {
            UserAccount userAccount = dc.Account.GetUserAccount();
            Console.WriteLine("UserId: " + userAccount.Id
                + "; FirstName: " + userAccount.FirstName
                + "; LastName: " + userAccount.LastName
                + "; E-mail: " + userAccount.Email);
        }

        private static void GetCustomerAccount() {
            CustomerAccount customerAccount = dc.Account.GetCustomerAccount();
            Console.WriteLine("CustomerId: " + customerAccount.Id
                + "; Name: " + customerAccount.Name
                + "; Accounts: " + customerAccount.AccountsUsed + "/" + customerAccount.AccountsLimit
                + "; Space: " + customerAccount.SpaceUsed + "/" + customerAccount.SpaceLimit);
        }

        private static void SetUserKeyPair() {
            dc.Account.SetUserKeyPair();
        }

        private static void CheckUserKeyPair() {
            bool encryptionPasswordIsValid = dc.Account.CheckUserKeyPairPassword();
            Console.WriteLine("Encryption password is valid: " + encryptionPasswordIsValid);
        }

        private static void DeleteUserKeyPair() {
            dc.Account.DeleteUserKeyPair();
        }

        #endregion

        #region DracoonClient.Nodes

        private static void ListRootNodes() {
            NodeList rootNodes = dc.Nodes.GetNodes();
            foreach (Node current in rootNodes.Items) {
                Console.WriteLine("NodeId: " + current.Id + "; NodeName: " + current.Name);
            }
        }

        private static void ListFilteredRootNodes() {
            GetNodesFilter getNodesFilter = new GetNodesFilter();
            getNodesFilter.AddNodeTypeFilter(GetNodesFilter.Type.EqualTo(NodeType.Room).Or().EqualTo(NodeType.Folder).Build());
            getNodesFilter.AddNameFilter(GetNodesFilter.Name.Contains("Test").Build());

            NodeList rootNodes = dc.Nodes.GetNodes(filter: getNodesFilter);
            foreach (Node current in rootNodes.Items) {
                Console.WriteLine("NodeId: " + current.Id + "; NodeName: " + current.Name);
            }
        }

        private static void CreateRoom() {
            List<long> roomAdminIds = new List<long> { 1 };

            CreateRoomRequest request = new CreateRoomRequest("TestRoom", adminUserIds: roomAdminIds, notes: "It's a test room creation.");
            Node createdRoomNode = dc.Nodes.CreateRoom(request);
            Console.WriteLine("Created room id: " + createdRoomNode.Id + "; Name: " + createdRoomNode.Name);
        }

        private static void UpdateRoom() {
            UpdateRoomRequest request = new UpdateRoomRequest(1, name: "RenamedTestRoom", notes: "Renamed the test room");
            Node updatedRoomNode = dc.Nodes.UpdateRoom(request);
            Console.WriteLine("Updated room id: " + updatedRoomNode.Id + "; Name: " + updatedRoomNode.Name);
        }

        private static void CreateFolder() {
            CreateFolderRequest request = new CreateFolderRequest(1, "TestFolder", "It's a test folder creation.");
            Node createdFolderNode = dc.Nodes.CreateFolder(request);
            Console.WriteLine("Created folder id: " + createdFolderNode.Id + "; Name: " + createdFolderNode.Name);
        }

        private static void DeleteNodes() {
            List<long> nodeIdsForDeletion = new List<long> { 1, 2 };

            DeleteNodesRequest request = new DeleteNodesRequest(nodeIdsForDeletion);
            dc.Nodes.DeleteNodes(request);
        }

        private static void CopyNodes() {
            List<CopyNode> nodeWhichWereCopied = new List<CopyNode> {
                new CopyNode(2),
                new CopyNode(3),
                new CopyNode(4)
            };

            CopyNodesRequest request = new CopyNodesRequest(1, nodeWhichWereCopied);
            Node resultingParentNode = dc.Nodes.CopyNodes(request);
            Console.WriteLine("New parent node id: " + resultingParentNode.Id + "; parent node childes: " + resultingParentNode.CountChildren);
        }

        private static void MoveNodes() {
            List<MoveNode> nodesWhichWereMoved = new List<MoveNode> {
                new MoveNode(5),
                new MoveNode(6),
                new MoveNode(7)
            };

            MoveNodesRequest request = new MoveNodesRequest(1, nodesWhichWereMoved);
            Node resultingParentNode = dc.Nodes.MoveNodes(request);
            Console.WriteLine("New parent node id: " + resultingParentNode.Id + "; parent node childes: " + resultingParentNode.CountChildren);
        }

        private static void SearchNodes() {
            NodeList searchedNodes = dc.Nodes.SearchNodes("Test", 0);

            foreach (Node current in searchedNodes.Items) {
                Console.WriteLine("SearchedNodeId: " + current.Id + "; NodeName: " + current.Name);
            }
        }

        private static void SearchNodesWithFilterAndSort() {
            SearchNodesFilter searchFilter = new SearchNodesFilter();
            searchFilter.AddNodeTypeFilter(SearchNodesFilter.Type.EqualTo(NodeType.File).Build());

            NodeList searchedNodes = dc.Nodes.SearchNodes("Test", 0, filter: searchFilter, sort: SearchNodesSort.Size.Ascending());

            foreach (Node current in searchedNodes.Items) {
                Console.WriteLine("SearchedNodeId: " + current.Id + "; NodeName: " + current.Name + "; Size: " + current.Size);
            }
        }

        private static void GetFavorites() {
            SearchNodesFilter favoriteFilter = new SearchNodesFilter();
            favoriteFilter.AddIsFavoriteFilter(SearchNodesFilter.IsFavorite.EqualTo(true).Build());

            NodeList favoriteNodes = dc.Nodes.SearchNodes("*", 0, filter: favoriteFilter);

            foreach (Node current in favoriteNodes.Items) {
                Console.WriteLine("SearchedNodeId: " + current.Id + "; NodeName: " + current.Name + "; isFavorite: " + current.IsFavorite);
            }
        }

        private static void UploadFile() {
            FileUploadRequest reqeust = new FileUploadRequest(1, "testFile.txt");
            FileStream stream = File.Open("C:\\temp\\testFile.txt", FileMode.Open);
            Node uploadedNode = dc.Nodes.UploadFile(Guid.NewGuid().ToString(), reqeust, stream, callback: new ULCallback());
        }

        private static void DownloadEncryptedFile() {
            dc.EncryptionPassword = ENCRYPTION_PASSWORD;
            Node node = dc.Nodes.GetNode(1);
            FileStream stream = File.Create("C:\\temp\\" + node.Name);
            dc.Nodes.DownloadFile(Guid.NewGuid().ToString(), node.Id, stream);
        }

        private static void DownloadFileAsync() {
            Node node = dc.Nodes.GetNode(1);
            FileStream stream = File.Create("C:\\temp\\" + node.Name);
            dc.Nodes.StartDownloadFileAsync(Guid.NewGuid().ToString(), node.Id, stream, new DLCallback());
        }

        private static void DownloadFile() {
            Node node = dc.Nodes.GetNode(1);
            FileStream stream = File.Create("C:\\temp\\" + node.Name);
            dc.Nodes.DownloadFile(Guid.NewGuid().ToString(), node.Id, stream, new DLCallback());
        }

        #endregion

        #region RecycleBin / Versioning

        private static void GetFileVersions() {
            RecycleBinItemList binItems = dc.Nodes.GetRecycleBinItems(1);
            foreach (RecycleBinItem current in binItems.Items) {
                Console.WriteLine("NodeName: " + current.Name + "; Versions: " + current.VersionsCount + "; LastDeletedNodeId: " + current.LastDeletedNodeId + "; ParentPath: " + current.ParentPath);
            }

            PreviousVersionList versionList = dc.Nodes.GetPreviousVersions(1, NodeType.File, "test.txt");
            foreach (PreviousVersion current in versionList.Items) {
                Console.WriteLine("NodeName: " + current.Name + "; Id: " + current.Id + "; ParentPath: " + current.ParentPath + "; DeletedAt: " + current.DeletedAt.ToString());
            }

            // Restore the last version of the node "test.txt"
            RestorePreviousVersionsRequest request = new RestorePreviousVersionsRequest(new List<long>() { versionList.Items[0].Id.Value });
            dc.Nodes.RestorePreviousVersion(request);
        }

        #endregion

        #region File Up/Download-Callbacks

        private class DLCallback : IFileDownloadCallback {
            private Dictionary<string, Stopwatch> requestTimings = new Dictionary<string, Stopwatch>();
            public void OnCanceled(string actionId) {
                requestTimings.Remove(actionId);
                Console.WriteLine("DLCallback -> " + "Download canceled: " + actionId);
            }

            public void OnFailed(string actionId, DracoonException occuredError) {
                requestTimings.Remove(actionId);
                Console.WriteLine("DLCallback -> " + "Download failed: " + actionId + " with: " + occuredError.Message);
            }

            public void OnFinished(string actionId) {
                Stopwatch watch;
                if (requestTimings.TryGetValue(actionId, out watch)) {
                    watch.Stop();
                    Console.WriteLine("DLCallback -> " + "Download finished: " + actionId + " (" + watch.Elapsed.ToString() + ")");
                    requestTimings.Remove(actionId);
                } else {
                    Console.WriteLine("DLCallback -> " + "Download finished: " + actionId);
                }
            }

            public void OnRunning(string actionId, long bytesDownloaded, long bytesTotal) {
                Console.WriteLine("DLCallback -> " + "Download progress for: " + actionId + " --> " + bytesDownloaded + "/" + bytesTotal);
            }

            public void OnStarted(string actionId) {
                Stopwatch newWatch = new Stopwatch();
                requestTimings.Add(actionId, newWatch);
                newWatch.Start();
                Console.WriteLine("DLCallback -> " + "Download started: " + actionId);
            }
        }

        private class ULCallback : IFileUploadCallback {
            private Dictionary<string, Stopwatch> requestTimings = new Dictionary<string, Stopwatch>();
            public void OnCanceled(string actionId) {
                requestTimings.Remove(actionId);
                Console.WriteLine("ULCallback -> " + "Upload canceled: " + actionId);
            }

            public void OnFailed(string actionId, DracoonException occuredError) {
                requestTimings.Remove(actionId);
                Console.WriteLine("ULCallback -> " + "Upload failed: " + actionId + " with: " + occuredError.Message);
            }

            public void OnFinished(string actionId, Node resultNode) {
                if (requestTimings.TryGetValue(actionId, out Stopwatch watch)) {
                    watch.Stop();
                    Console.WriteLine("ULCallback -> " + "Upload finished: " + actionId + " | New node id is " + resultNode.Id + " and name " + resultNode.Name + " (" + watch.Elapsed.ToString() + ")");
                    requestTimings.Remove(actionId);
                } else {
                    Console.WriteLine("ULCallback -> " + "Upload finished: " + actionId);
                }
            }

            public void OnRunning(string actionId, long bytesUploaded, long bytesTotal) {
                Console.WriteLine("ULCallback -> " + "Upload progress for: " + actionId + " --> " + bytesUploaded + "/" + bytesTotal);
            }

            public void OnStarted(string actionId) {
                Stopwatch newWatch = new Stopwatch();
                requestTimings.Add(actionId, newWatch);
                newWatch.Start();
                Console.WriteLine("ULCallback -> " + "Upload started: " + actionId);
            }
        }

        #endregion
    }
}
