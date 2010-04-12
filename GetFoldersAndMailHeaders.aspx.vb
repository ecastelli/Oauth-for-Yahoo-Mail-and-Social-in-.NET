Imports System.Text.RegularExpressions
Imports System.Configuration



Partial Class GetFoldersAndMailHeaders
    Inherits System.Web.UI.Page
    Dim mintRequestID As Integer
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        getFoldersAndMailHeaders()

    End Sub


    'gets a list of the user's folders than traverses each folder and gets the headers for mail using oauth
    Public Sub getFoldersAndMailHeaders()
        Try

        
            Dim objFolderCol As New Collection
            Dim objYmail As New YAPI
            Dim colMailFolders As New Collection
            Dim objMailFolder As clsMailFolder
            Dim strMessageIdsToRetrieve() As String
            Dim dblReceivedDate As Double
            Dim dtReceivedDate As DateTime
            Dim strFromEmail As String = ""
            Dim strSubject As String = ""
            Dim strEmailSubject As String = ""

            'create a new instance of YAPI as defined in clsYmail.vb
            'note YAPI inherits  the web service Ymail.ymws.   We have to inherit this class so we can override the web request to add our Oauth headers.

            'set our soap url and the token and secret which are required for making the calls.
            objYmail.Url = System.Configuration.ConfigurationManager.AppSettings("YahooSoapURL")
            objYmail.strAuthTokenSecret = Session("oauth_token_secret")
            objYmail.strAuthToken = Session("oauth_token")

            'if you put a breakpoint in clsYmail.vb at GetWebRequest you will see how the oath headers are added to the soap request
            Dim objListFoldersResponse = objYmail.ListFolders(New Ymail.ListFolders())
            Dim objFolder As Ymail.FolderData

            'get a list of the user's folders.  
            For Each objFolder In objListFoldersResponse.folder
                objMailFolder = New clsMailFolder
                objMailFolder.FolderID = objFolder.folderInfo.fid
                objMailFolder.FolderName = objFolder.folderInfo.name
                objMailFolder.MailQTY = objFolder.total
                colMailFolders.Add(objMailFolder)
            Next objFolder


            'now that we have our list of folders, let's get some mail from each folder.
            For Each objMailFolder In colMailFolders
                'we use the list messages function to get a list of the message ids in the folder
                Dim objListRequest As New Ymail.ListMessages()
                Dim inti As Integer = 0
                objListRequest.fid = objMailFolder.FolderID
                objListRequest.sortKey = Ymail.SortKey.date
                objListRequest.sortOrder = Ymail.SortOrder.down
                objListRequest.startMid = 0

                'the number of messages to get are set in the web config.  you can up the number of messages to read in the config.  
                objListRequest.numInfo = System.Configuration.ConfigurationManager.AppSettings("MaxMailScanQty")
                objListRequest.numInfoSpecified = True
                objListRequest.startMidSpecified = True
                objListRequest.numMid = System.Configuration.ConfigurationManager.AppSettings("MaxMailScanQty")
                objListRequest.numMidSpecified = True

                Dim objListResponse As Ymail.ListMessagesResponse
                objListResponse = objYmail.ListMessages(objListRequest)

                'if we have messages to get.  let's loop through each message id and get the headers.
                If IsNothing(objListResponse.mid) = False Then
                    strMessageIdsToRetrieve = objListResponse.mid

                    'now get our message headers
                    Dim objGetRawHeaderRequest As New Ymail.GetMessageRawHeader
                    objGetRawHeaderRequest.fid = objMailFolder.FolderID
                    objGetRawHeaderRequest.mid = strMessageIdsToRetrieve
                    Dim objGetRawHeaderResponse As Ymail.GetMessageRawHeaderResponse
                    objGetRawHeaderResponse = objYmail.GetMessageRawHeader(objGetRawHeaderRequest)
                    For inti = 0 To objGetRawHeaderResponse.rawheaders.Length - 1
                        'data pulled from header request
                        dblReceivedDate = objListResponse.messageInfo(inti).receivedDate

                        'convert date
                        dtReceivedDate = DateAdd(DateInterval.Second, dblReceivedDate, CDate("01/01/1970 12:00:00 AM"))

                        strFromEmail = objListResponse.messageInfo(inti).from.email
                        strSubject = objListResponse.messageInfo(inti).subject

                        'here's our mail info
                        Response.Write("Folder: " & objMailFolder.FolderName & ", Mail Received on: " & dtReceivedDate & ", Sent From: " & strFromEmail & ", Subject: " & strSubject & "<br/>")


                    Next inti


                End If

            Next
        Catch ex As Exception
            Response.Write("Something bad happened getting folders and mail. " & ex.ToString)
        End Try

    End Sub
End Class
