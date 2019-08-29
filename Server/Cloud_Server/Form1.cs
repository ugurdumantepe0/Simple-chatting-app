using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.Threading;
using System.Net.Sockets;
using System.Net;
using System.Text.RegularExpressions;

namespace Cloud_Server
{
    public partial class Form1 : Form
    {
        private struct Client
        {
            public string NickName;
            public Socket UserSocket;
        };

        private struct Process
        {
            private Client client;
            private string inProgress;
        }

        private const int BYTE_LENGTH = 2048;
        private const int FILE_BUFFER_LENGTH = 8192;

        private const string SERVER_OFFLINE = "START SERVER";
        private const string SERVER_ONLINE = "STOP SERVER";

        private bool _connected = true;
        private string _directory;

        Thread _thrAccept;
        Socket _socket;

        private List<Socket> _socketArray = new List<Socket>();
        private List<Client> _clientArray = new List<Client>();
        private List<Process> _processArray = new List<Process>();

        // server error messages
        private const string PORT_ERROR = "Please enter a numeric port number.";
        private const string PATH_ERROR = "Please select a path for uploaded files.";
        private const string SERVER_START_LOG = "Server is now running. Listening for new clients...";
        private const string SERVER_STOP_LOG = "Server was stopped manually.";
        private const string CLIENT_UPLOAD_MSG = " has started an upload operation.";
        private const string CLIENT_REUPLOAD_MSG = " has started an re-upload operation.";
        private const string REMOVE_ALL_CLIENTS_MSG = " client(s) were notified and disconnected, server stopped.";
        private const string NO_USERS_TO_DSC_MSG = "There were no connected clients.";
        private const string USER_CONNECTED_MSG = " has connected to the server.";
        private const string USER_DSC_MSG = " has disconnected from the server.";

        // success / fail messages
        private const string UPLOAD_SUCCESS_BACKUP_DELETED = " has re-uploaded a file, backup file has deleted.";
        private const string UPLOAD_SUCCESS_BACKUP_FAILED_TO_DELETE = " has re-uploaded a file, backup file could NOT deleted.";
        private const string UPLOAD_FAILED_BACKUP_SUCCESS = " has failed to re-upload a file, backup file restored.";
        private const string UPLOAD_FAILED_BACKUP_FAILED = " has failed to re-upload a file, backup file could NOT be restored.";
        private const string UPLOAD_FAILED_BACKUP_WAS_DELETED = " has failed to re-upload a file, backup file could NOT be restored, backup file was NOT found, the failed upload file has deleted.";
        private const string UPLOAD_SUCCESS_BACKUP_WAS_DELETED = " has re-uploaded a file, backup file was NOT found.";
        private const string UPLOAD_FAILED = " failed to upload a file.";
        private const string UPLOAD_SUCCESS = " has uploaded a file.";

        // server side commands (server sends client)
        private const string SERVER_SHUTDOWN_COMMAND = "_DSC_|_STOPPED_MANUALLY_|_END_OF_MSG_";
        private const string SERVER_INTERNAL_ERROR_COMMAND = "_DSC_|_CLOSED_FORCIBLY_|_END_OF_MSG_";
        private const string USER_EXISTS_COMMAND = "_DSC_|_ALREADYEXISTS_|_END_OF_MSG_";
        private const string NO_FILES_TO_RETURN_COMMAND = "_NO_FILES_FOUND_|_END_OF_MSG_";
        private const string FILE_NOT_FOUND_COMMAND = "_FILE_NOT_FOUND_|_END_OF_MSG_";
        private const string FILE_UPLOAD_TO_CLIENT_COMMAND = "_UPLOADING_A_FILE_|";
        private const string FILE_TO_DELETE_NOT_FOUND_COMMAND = "_FILE_TO_DELETE_NOT_FOUND_|_END_OF_MSG_";
        private const string FILE_TO_RENAME_NOT_FOUND_COMMAND = "_FILE_TO_RENAME_NOT_FOUND_|_END_OF_MSG_";
        private const string DELETE_SUCCESS_COMMAND = "_FILE_DELETED_|_END_OF_MSG_";
        private const string RENAME_SUCCESS_COMMAND = "_FILE_RENAMED_|_END_OF_MSG_";
        private const string FILE_CANNOT_BE_ACCESSED_TO_DELETE_COMMAND = "_FILE_CANNOT_BE_ACCESSED_TO_DELETE_|_END_OF_MSG_";
        private const string FILE_CANNOT_BE_ACCESSED_TO_RENAME_COMMAND = "_FILE_CANNOT_BE_ACCESSED_TO_RENAME_|_END_OF_MSG_";
        private const string NEW_FILE_NAME_ALREADY_EXISTS_COMMAND = "_NEW_FILE_NAME_ALREADY_EXISTS_|_END_OF_MSG_";

        // client side commands (client sends server)
        private const string NEW_USER_COMMAND = "_ADD_NEW_USER_";
        private const string END_MSG_COMMAND = "_END_OF_MSG_";
        private const string USER_DC_COMMAND = "_DSC_";
        private const string USER_FILE_UPLOAD_COMMAND = "_FILE_UPLOAD_REQUEST_";
        private const string GET_ALL_FILES_COMMAND = "_GET_ALL_FILES_";
        private const string DOWNLOAD_SELECTED_FILE_COMMAND = "_DOWNLOAD_FILE_";
        private const string DELETE_SELECTED_FILE_COMMAND = "_DELETE_FILE_";
        private const string RENAME_SELECTED_FILE_COMMAND = "_RENAME_FILE_";

        public Form1()
        {
            CheckForIllegalCrossThreadCalls = false;
            InitializeComponent();
        }

        private void startButton_Click(object sender, EventArgs e)
        {
            // CHECK IF PORT BOX HAS NUMERIC TEXT
            if (!Regex.IsMatch(portBox.Text, @"^\d+$"))
            {
                MessageBox.Show(PORT_ERROR);
            }
            else if (startButton.Text.Equals(SERVER_OFFLINE))
            {
                // CHOOSE CLIENT UPLOAD LOCATION
                FolderBrowserDialog fbd = new FolderBrowserDialog();
                DialogResult result = DialogResult.Cancel;
                while (result != DialogResult.OK)
                {
                    result = fbd.ShowDialog();
                    if (result != DialogResult.OK)
                        MessageBox.Show(PATH_ERROR);
                }
                _directory = fbd.SelectedPath;

                // clear lists for safety
                onlineList.Items.Clear();
                _clientArray.Clear();
                _socketArray.Clear();

                _connected = true;
                portBox.ReadOnly = true;

                _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                _socket.Bind(new IPEndPoint(IPAddress.Any, Convert.ToInt32(portBox.Text)));
                _socket.Listen(3);

                _thrAccept = new Thread(new ThreadStart(Accept));
                _thrAccept.Start();

                startButton.Text = SERVER_ONLINE;

                reportBox.AppendText(GetTime() + SERVER_START_LOG + Environment.NewLine);
            }

            else
            {
                // stop listening
                _socket.Close();
                _connected = false;

                RemoveAllClients(SERVER_SHUTDOWN_COMMAND);

                startButton.Text = SERVER_OFFLINE;
                portBox.ReadOnly = false;

                reportBox.AppendText(GetTime() + SERVER_STOP_LOG + Environment.NewLine);
            }
        }

        // ACCEPT NEW CLINETS (LISTEN)
        private void Accept()
        {
            while (_connected)
            {
                try
                {
                    Socket n = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    n = _socket.Accept();
                    _socketArray.Add(n);
                    Thread thrReceive = new Thread(new ParameterizedThreadStart(Receive));
                    thrReceive.Start(n);
                }
                catch (Exception)
                {
                    ServerError_Shutdown();
                }
            }
        }

        private string GetTime()
        {
            return "[" + DateTime.Now.ToString("HH:mm") + "] ";
        }

        // SHUTDOWN SERVER AND NOTIFY ALL ONLINE CLINETS
        private void ServerError_Shutdown()
        {
            _connected = false;

            // clear client and online lists
            _clientArray.Clear();
            onlineList.Items.Clear();

            // NOTIFY CLIENTS
            if (_socketArray.Count != 0)
            {
                try
                {
                    foreach (Socket soc in _socketArray)
                        if (soc.Connected)
                            soc.Send(Encoding.Default.GetBytes(SERVER_INTERNAL_ERROR_COMMAND));
                }
                catch (Exception)
                {
                    //  MessageBox.Show("EX_INSIDE_Accept");
                }
                finally
                {
                    // shutdown open sockets
                    foreach (Socket soc in _socketArray)
                    {
                        if (soc.Connected)
                        {
                            soc.Shutdown(SocketShutdown.Both);
                            //soc.Disconnect(true);
                        }
                        soc.Close();
                    }

                    // clear socket list         
                    _socketArray.Clear();
                }
            }

            portBox.ReadOnly = false;
            startButton.Text = SERVER_OFFLINE;
        }

        // REMOVE A CLIENT
        private void RemoveClient(Socket soc)
        {
            // remove socket from socket array and close it
            if (soc.Connected)
                soc.Shutdown(SocketShutdown.Both);
            soc.Close();

            if (_socketArray.Contains(soc))
                _socketArray.Remove(soc);

            // search client in client array
            if (_clientArray.Count != 0)
            {
                foreach (Client c in _clientArray)
                {
                    if (c.UserSocket.Equals(soc)) // user found
                    {
                        // server log
                        string nick = c.NickName;
                        reportBox.AppendText(GetTime() + c.NickName + USER_DSC_MSG + Environment.NewLine);

                        // remove client from client array and online list 
                        if (onlineList.Items.Contains(c.NickName))
                            onlineList.Items.Remove(c.NickName);
                        _clientArray.Remove(c);
                        break;
                    }
                }
            }
        }

        // NOTIFIES ALL CONNECTED CLIENTS WITH THE RESPONSE
        // CLEARS CLIENT LISTS
        private void RemoveAllClients(string response)
        {
            // NOTIFY CLIENTS
            try
            {
                if (_socketArray.Count != 0)
                {
                    foreach (Socket soc in _socketArray)
                    {
                        if (soc.Connected)
                        {
                            soc.Send(Encoding.Default.GetBytes(response));
                            soc.Shutdown(SocketShutdown.Both);
                        }
                        soc.Close();
                    }
                    reportBox.AppendText(GetTime() + _socketArray.Count + REMOVE_ALL_CLIENTS_MSG + Environment.NewLine);
                }
                else
                {
                    reportBox.AppendText(GetTime() + NO_USERS_TO_DSC_MSG + Environment.NewLine);
                }
            }
            catch (Exception)
            {
                //MessageBox.Show("EX_FORM_CLOSE");
            }
            finally
            {
                onlineList.Items.Clear();
                _clientArray.Clear();
                _socketArray.Clear();
            }
        }

        // REMOVE _END_OF_MSG_ COMMAND
        private void removeEND(ref string msg)
        {
            msg = msg.Substring(0, msg.Length - 13);
        }

        //////////////////////// LISTEN CLIENT THREAD ////////////////////////////
        private void Receive(object s)
        {
            Socket n = (Socket)s;
            while (_connected && n.Connected)
            {
                byte[] buffer = new byte[BYTE_LENGTH];
                try
                {
                    if (_connected)
                        n.Receive(buffer);
                }
                catch (Exception)
                {
                    if (_connected)
                        RemoveClient(n);
                }

                // first received string
                string receivedData = Encoding.Default.GetString(buffer).TrimEnd('\0'); // remove null characters
                if (!string.IsNullOrEmpty(receivedData))
                {
                    //                   reportBox.AppendText("receivedData: " + receivedData + Environment.NewLine); //////////////
                    // received = COMMAND | USERNAME | DATA | _END_OF_MSG_
                    removeEND(ref receivedData);
                    //                   reportBox.AppendText("removeEnd: " + receivedData + Environment.NewLine); //////////////
                    // received = COMMAND | USERNAME | DATA
                    string command = receivedData.Substring(0, receivedData.IndexOf('|'));
                    //                   reportBox.AppendText("command: " + command + Environment.NewLine); //////////////////
                    receivedData = receivedData.Substring(receivedData.IndexOf('|') + 1);
                    //                   reportBox.AppendText("username | data: " + receivedData + Environment.NewLine); ////////////////////
                    // received = USERNAME | DATA
                    string userName = receivedData.Substring(0, receivedData.IndexOf('|'));
                    //                   reportBox.AppendText("username: " + userName + Environment.NewLine); ////////////////////
                    receivedData = receivedData.Substring(receivedData.IndexOf('|') + 1);
                    //                   reportBox.AppendText("data: " + receivedData + Environment.NewLine); ////////////////////
                    // received =  DATA



                    if (command.Equals("_PING_"))
                    {
                        // do nothing ..
                    }
                    //********* NEW CLIENT
                    // _ADD_NEW_USER_|nickName
                    else if (command.Equals(NEW_USER_COMMAND))
                    {
                        bool clientDoesNotExist = true;

                        // check client list for existing username
                        foreach (Client old in _clientArray)
                        {
                            if (old.NickName.Equals(userName))
                                clientDoesNotExist = false;
                        }

                        if (clientDoesNotExist)
                        {
                            Client newCient = new Client();

                            newCient.NickName = userName;
                            newCient.UserSocket = n;

                            // add cliet to online and client lists
                            onlineList.Items.Add(newCient.NickName);
                            _clientArray.Add(newCient);

                            reportBox.AppendText(GetTime() + newCient.NickName + USER_CONNECTED_MSG + Environment.NewLine);
                        }
                        else // if username already exists, notify user and disconnect
                        {
                            try
                            {
                                if (n.Connected)
                                    n.Send(Encoding.Default.GetBytes(USER_EXISTS_COMMAND));
                            }
                            finally
                            {
                                if (n.Connected)
                                    n.Shutdown(SocketShutdown.Both);
                                n.Close();
                                if (_socketArray.Contains(n))
                                    _socketArray.Remove(n);
                            }
                        }
                    }

                    //********* CLIENT HAVE DISCONNECTED
                    else if (command.Equals(USER_DC_COMMAND))
                    {
                        RemoveClient(n);
                    }

                    // CLIENT REQUESTS A FILE DOWNLOAD // ERROR FREE
                    else if (command.Equals(DOWNLOAD_SELECTED_FILE_COMMAND))
                    {
                        string fileName = receivedData;
                        string userDirectory = Path.Combine(_directory, userName);
                        string filePath = Path.Combine(userDirectory, fileName);
                        string backupFilePath = Path.Combine(userDirectory, "backup", fileName);

                        if (!Directory.Exists(userDirectory))
                        {
                            reportBox.AppendText(GetTime() + userName + " has requested to download " + fileName + ": USER DIRECTORY NOT FOUND." + Environment.NewLine);
                            try
                            {
                                n.Send(Encoding.Default.GetBytes(FILE_NOT_FOUND_COMMAND));
                                SendAvailableFiles(n, userName);
                            }
                            catch (Exception)
                            {
                                RemoveClient(n);
                            }
                        }
                        else
                        {
                            if (File.Exists(filePath))
                            {
                                reportBox.AppendText(GetTime() + userName + " has requested to download " + fileName + ": SENDING FILE..." + Environment.NewLine);
                                UploadFileToClient(n, userName, filePath);
                            }
                            else if (File.Exists(backupFilePath))
                            {
                                reportBox.AppendText(GetTime() + userName + " has requested to download " + fileName + ": FILE WAS FOUND IN BACKUP FOLDER, SENDING FILE..." + Environment.NewLine);
                                UploadFileToClient(n, userName, backupFilePath);
                            }
                            else
                            {
                                reportBox.AppendText(GetTime() + userName + " has requested to download " + fileName + ": FILE NOT FOUND." + Environment.NewLine);
                                try
                                {
                                    n.Send(Encoding.Default.GetBytes(FILE_NOT_FOUND_COMMAND));
                                    SendAvailableFiles(n, userName);
                                }
                                catch (Exception)
                                {
                                    RemoveClient(n);
                                }
                            }
                        }
                    }

                    // CLIENT REQUEST ALL AVAILABLE FILE NAMES
                    else if (command.Equals(GET_ALL_FILES_COMMAND))
                    {
                        reportBox.AppendText(GetTime() + userName + " has requested thier available files." + Environment.NewLine);
                        SendAvailableFiles(n, userName);
                    }

                    // CLIENT REQUEST TO DELETE A FILE
                    else if (command.Equals(DELETE_SELECTED_FILE_COMMAND))
                    {
                        string fileName = receivedData;
                        reportBox.AppendText(GetTime() + userName + " has requested to delete: " + fileName + Environment.NewLine);

                        string userDirectory = Path.Combine(_directory, userName);                 
                        string filePath = Path.Combine(userDirectory, fileName);

                        if (File.Exists(filePath))
                        {
                            try
                            {
                                System.IO.File.Delete(filePath);
                                Thread.Sleep(40);
                                reportBox.AppendText(GetTime() + userName + " has successfully deleted: " + fileName +
                                                     Environment.NewLine);
                                n.Send(Encoding.Default.GetBytes(DELETE_SUCCESS_COMMAND));

                            }
                            catch (System.IO.IOException e)
                            {
                                reportBox.AppendText(GetTime() + userName + " has failed to delete: " + fileName +
                                                     Environment.NewLine);
                                try
                                {
                                    n.Send(Encoding.Default.GetBytes(FILE_CANNOT_BE_ACCESSED_TO_DELETE_COMMAND));
                                }
                                catch (Exception)
                                {
                                    RemoveClient(n);
                                }
                                finally
                                {
                                    MessageBox.Show(e.Message);
                                }
                            }
                            catch (Exception)
                            {
                                RemoveClient(n);
                            }
                        }
                        else
                        {
                            try
                            {
                                reportBox.AppendText(GetTime() + fileName + " not found in " + userName + "'s directory." + Environment.NewLine);
                                n.Send(Encoding.Default.GetBytes(FILE_TO_DELETE_NOT_FOUND_COMMAND));
                            }
                            catch (Exception)
                            {
                                RemoveClient(n);
                            }
                        }
                    }

                    // CLIENT WANTS TO RENAME A FILE
                    else if (command.Equals(RENAME_SELECTED_FILE_COMMAND))
                    {
                        string[] fileNames = receivedData.Split('/');
                        string oldFileName = fileNames[0];
                        string newFileName = fileNames[1];

                        reportBox.AppendText(GetTime() + userName + " has requested to rename: " + oldFileName + Environment.NewLine);

                        string userDirectory = Path.Combine(_directory, userName);
                        string newFilePath = Path.Combine(userDirectory, newFileName);
                        string oldFilePath = Path.Combine(userDirectory, oldFileName);


                        if (!File.Exists(oldFilePath))
                        {
                            try
                            {
                                reportBox.AppendText(GetTime() + oldFileName + " not found in " + userName + "'s directory." + Environment.NewLine);
                                n.Send(Encoding.Default.GetBytes(FILE_TO_RENAME_NOT_FOUND_COMMAND));
                            }
                            catch (Exception)
                            {
                                RemoveClient(n);
                            }
                        }
                        else if (File.Exists(newFilePath))
                        {
                            try
                            {
                                reportBox.AppendText(GetTime() + newFileName + " already exists in " + userName + "'s directory." + Environment.NewLine);
                                n.Send(Encoding.Default.GetBytes(NEW_FILE_NAME_ALREADY_EXISTS_COMMAND));
                            }
                            catch (Exception)
                            {
                                RemoveClient(n);
                            }
                        }
                        else
                        {
                            try
                            {
                                System.IO.File.Move(oldFilePath, newFilePath);
                                Thread.Sleep(40);
                                reportBox.AppendText(GetTime() + userName + " has successfully renamed: " + oldFileName + " to " + newFileName +
                                                     Environment.NewLine);
                                n.Send(Encoding.Default.GetBytes(RENAME_SUCCESS_COMMAND));

                            }
                            catch (System.IO.IOException e)
                            {
                                reportBox.AppendText(GetTime() + userName + " has failed to rename: " + oldFileName +
                                                     Environment.NewLine);
                                try
                                {
                                    n.Send(Encoding.Default.GetBytes(FILE_CANNOT_BE_ACCESSED_TO_RENAME_COMMAND));
                                }
                                catch (Exception)
                                {
                                    RemoveClient(n);
                                }
                                finally
                                {
                                    MessageBox.Show(e.Message);
                                }
                            }
                            catch (Exception)
                            {
                                RemoveClient(n);
                            }
                        }
                    }

                    // CLINET WANTS TO UPLOAD A FILE
                    else if (command.Equals(USER_FILE_UPLOAD_COMMAND))
                    {
                        bool reUpload = false; // check to see if it's a re-upload
                        string backupFolder = string.Empty;
                        string backupPath = string.Empty;

                        int noOfpackets = Convert.ToInt32(receivedData.Substring(0, receivedData.IndexOf("|")));
                        string fileName = receivedData.Substring(receivedData.IndexOf("|") + 1);

                        string userDirectory = Path.Combine(_directory, userName); // user folder                  
                        string filePath = Path.Combine(userDirectory, fileName); // file path to write data

                        // create user folder if not exists
                        if (!Directory.Exists(userDirectory))
                            Directory.CreateDirectory(userDirectory);

                        // RE-UPLOAD CASE
                        if (File.Exists(filePath))
                        {
                            reUpload = true;
                            backupFolder = Path.Combine(userDirectory, "backup");
                            backupPath = Path.Combine(backupFolder, fileName);
                            if (!Directory.Exists(backupFolder))
                                Directory.CreateDirectory(backupFolder);
                            if (!File.Exists(backupPath))        // if a backup doesn't exist, move file to backup folder
                                File.Move(filePath, backupPath);
                        }

                        if (reUpload)
                            reportBox.AppendText(GetTime() + userName + CLIENT_REUPLOAD_MSG + Environment.NewLine);
                        else
                            reportBox.AppendText(GetTime() + userName + CLIENT_UPLOAD_MSG + Environment.NewLine);

                        Stream fileStream = null;

                        try
                        {
                            fileStream = new FileStream(filePath, FileMode.Create, FileAccess.ReadWrite);

                            // Buffer for reading data  
                            byte[] bytes = new byte[FILE_BUFFER_LENGTH];

                            int length = 0;

                            for (int i = 0; i < noOfpackets; i++)
                            {
                                // RECEIVE CHUNK FROM CLIENT ***
                                length = n.Receive(bytes, 0, bytes.Length, SocketFlags.None);
                                Thread.Sleep(10);
                                fileStream.Write(bytes, 0, length);
                            }

                            if (fileStream != null)
                                fileStream.Close();

                            if (reUpload && !backupPath.Equals(string.Empty) && System.IO.File.Exists(backupPath))
                            {
                                try
                                {
                                    System.IO.File.Delete(backupPath);
                                    reportBox.AppendText(GetTime() + userName + UPLOAD_SUCCESS_BACKUP_DELETED + Environment.NewLine);
                                }
                                catch (System.IO.IOException e)
                                {
                                    reportBox.AppendText(GetTime() + userName + UPLOAD_SUCCESS_BACKUP_FAILED_TO_DELETE + Environment.NewLine);
                                    MessageBox.Show(e.Message);
                                }
                            }
                            else if (reUpload)
                            {
                                reportBox.AppendText(GetTime() + userName + UPLOAD_SUCCESS_BACKUP_WAS_DELETED + Environment.NewLine);
                            }
                            else
                            {
                                reportBox.AppendText(GetTime() + userName + UPLOAD_SUCCESS + Environment.NewLine);
                            }
                        }
                        catch (Exception)
                        {
                            if (fileStream != null)
                                fileStream.Close(); // unlock incomplete file
                            if (System.IO.File.Exists(filePath))
                            {
                                try
                                {
                                    System.IO.File.Delete(filePath); // remove incomplete upload
                                }
                                catch (Exception) { }
                            }

                            if (reUpload && !backupPath.Equals(string.Empty) && System.IO.File.Exists(backupPath))
                            {
                                try
                                {
                                    System.IO.File.Move(backupPath, filePath);  // move backup folder to original position
                                    reportBox.AppendText(GetTime() + userName + UPLOAD_FAILED_BACKUP_SUCCESS + Environment.NewLine);
                                }
                                catch (System.IO.IOException e)
                                {
                                    reportBox.AppendText(GetTime() + userName + UPLOAD_FAILED_BACKUP_FAILED + Environment.NewLine);
                                    MessageBox.Show(e.Message);
                                }
                            }
                            else if (reUpload)
                            {
                                reportBox.AppendText(GetTime() + userName + UPLOAD_FAILED_BACKUP_WAS_DELETED + Environment.NewLine);
                            }
                            else
                            {
                                reportBox.AppendText(GetTime() + userName + UPLOAD_FAILED + Environment.NewLine);
                            }
                            RemoveClient(n);
                        }
                        finally
                        {
                            if (Directory.Exists(backupFolder) && IsDirectoryEmpty(backupFolder))
                            {
                                try
                                {
                                    Directory.Delete(backupFolder, false);
                                }
                                catch (Exception) { }
                            }
                        }
                    }
                }
            }
        }

        void UploadFileToClient(Socket n, string userName, string filePath)
        {
            FileStream fileStream = null;
            try
            {
                fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read); // open file stream for file with read access
                                                                                       // calculate the number of chunks it will take to send the file
                int noOfPackets = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(fileStream.Length) / Convert.ToDouble(FILE_BUFFER_LENGTH)));
                int totalLength = (int)fileStream.Length;
                // notify client
                n.Send(Encoding.Default.GetBytes(FILE_UPLOAD_TO_CLIENT_COMMAND + noOfPackets + "|" + END_MSG_COMMAND));
                Thread.Sleep(20);

                // start sending chunks
                for (int i = 0; i < noOfPackets && _connected; i++)
                {
                    int currentPacketLength;

                    if (totalLength > FILE_BUFFER_LENGTH)
                    {
                        currentPacketLength = FILE_BUFFER_LENGTH;
                        totalLength = totalLength - currentPacketLength;
                    }
                    else
                        currentPacketLength = totalLength;

                    byte[] bytesToSend = new byte[currentPacketLength];
                    fileStream.Read(bytesToSend, 0, currentPacketLength);
                    // SEND CHUNK TO CLIENT ***
                    n.Send(bytesToSend, 0, (int)bytesToSend.Length, SocketFlags.None);
                }
                if(_connected)
                    reportBox.AppendText(GetTime() + userName + " has successfully downloaded: " + Path.GetFileName(filePath) + Environment.NewLine);
                else
                    reportBox.AppendText(GetTime() + userName + " has failed to download: " + Path.GetFileName(filePath) + Environment.NewLine);
            }
            catch (Exception)
            {
                reportBox.AppendText(GetTime() + userName + " has failed to download: " + Path.GetFileName(filePath) + Environment.NewLine);
                RemoveClient(n);
            }
            finally
            {
                // close file stream
                if (fileStream != null)
                    fileStream.Close();
            }
        }

        // SEND AVAILABLE FILES FOR userName TO SOCKET n
        void SendAvailableFiles(Socket n, string userName)
        {
            try
            {
                string userDirectory = Path.Combine(_directory, userName); // user folder
                n.Send(Encoding.Default.GetBytes("_REMOVE_ALL_FILES_" + "|" + END_MSG_COMMAND));

                if (!Directory.Exists(userDirectory))
                    n.Send(Encoding.Default.GetBytes(NO_FILES_TO_RETURN_COMMAND));
                else
                {
                    DirectoryInfo d = new DirectoryInfo(userDirectory);
                    FileInfo[] Files = d.GetFiles(); // Get all files in directory

                    int fileCount = Files.Length;
                    if (fileCount > 0)
                    {
                        foreach (FileInfo file in Files)
                        {
                            string fileData = "_ADD_NEW_FILE_";
                            string date = file.CreationTime.Day + "/" + file.CreationTime.Month + "/" +
                                          file.CreationTime.Year;
                            string hour = file.CreationTime.Hour.ToString();
                            if (hour.Length == 1)
                                hour = "0" + hour;
                            string minute = file.CreationTime.Minute.ToString();
                            if (minute.Length == 1)
                                minute = "0" + minute;
                            string time = hour + ":" + minute;
                            fileData += "|" + file.Name + "*" + file.Length + "*" + date + " " + time + "*Own File";
                            fileData += "|" + END_MSG_COMMAND;
                            Thread.Sleep(25); // wait for client to read the buffer
                            n.Send(Encoding.Default.GetBytes(fileData));
                        }
                    }
                    else
                        n.Send(Encoding.Default.GetBytes(NO_FILES_TO_RETURN_COMMAND));
                }
            }
            catch (IOException)
            {
                reportBox.AppendText(GetTime() + "FAILED READ DIRECTORY FOR: " + userName + Environment.NewLine);
            }
            catch (Exception)
            {
                reportBox.AppendText(GetTime() + userName + " failed to get file data." + Environment.NewLine);
                RemoveClient(n);
            }
        }

        // check if a folder is empty
        public bool IsDirectoryEmpty(string path)
        {
            return !Directory.EnumerateFileSystemEntries(path).Any();
        }

        // AUTO SCROLL FOR REPORT BOX
        private void reportBox_TextChanged(object sender, EventArgs e)
        {
            reportBox.SelectionStart = reportBox.Text.Length;
            reportBox.ScrollToCaret();
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            try
            {
                if (_socket != null)
                    _socket.Close();
            }
            finally
            {
                _connected = false;
                RemoveAllClients(SERVER_SHUTDOWN_COMMAND);
            }
        }
    }
}
