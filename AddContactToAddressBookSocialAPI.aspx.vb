Imports System.Net
Imports System.Xml

Partial Class AddContactToAddressBookSocialAPI
    Inherits System.Web.UI.Page

    Protected Sub cmdAddToContacts_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles cmdAddToContacts.Click

        'uses the Yahoo! Social API to add a contact to the users address book.
        'You must get a consumer token before calling this API.  Don't try this without authenticating first or it will fail
        Dim postData As String
        Dim DATA As Byte()
        Dim encoding As New System.Text.ASCIIEncoding
        Dim objResponse As HttpWebResponse
        Dim strUserID As String
        Dim stream As System.IO.Stream

        'these session variables are populated on the auth.aspx page after the user links to your app.
        Dim strAuthToken As String = Session("oauth_token")
        Dim strAuthTokenSecret As String = Session("oauth_token_secret")

        Try


            If txtEmail.Text = "" Or txtName.Text = "" Then
                Response.Write("Please enter both the email address and name to test.")
                Exit Sub
            End If



            'step 1 - get users guid via get
            'in order to call the social api for contacts, we must determine what the current users GUID is.  We do this through and http get to the social guid url
            Dim uri As New Uri(System.Configuration.ConfigurationManager.AppSettings("YahooSocialGUIDUrl"))
            Dim objWebRequest = buildWebRequest(strAuthToken, strAuthTokenSecret, uri, "GET")
            objWebRequest.Method = "GET"

            'get the response from the server
            objResponse = objWebRequest.GetResponse
            Dim objStreamReader As System.IO.Stream
            objStreamReader = objResponse.GetResponseStream()
            Dim strResult As String
            Dim objStreamResult As New System.IO.StreamReader(objStreamReader)

            'by default the respoinse returns the guid details in xml.  You could switch this to JSON if you like by passing a param to indicate the format of the response. 
            'see http://developer.yahoo.com/social/rest_api_guide/introspective-guid-resource.html for details
            strResult = objStreamResult.ReadToEnd
            Dim doc As New XmlDocument
            Dim xmNodeList As XmlNodeList
            Dim xmNodeElement As XmlElement
            doc.LoadXml(strResult)
            xmNodeList = doc.GetElementsByTagName("guid")
            xmNodeElement = xmNodeList(0)


            'here is the users guid which we need for adding a contact to their address book
            strUserID = xmNodeElement.InnerText
            objStreamReader.Close()
            objStreamResult.Close()
            objResponse.Close()


            'step 2 - add contact to users contact list via xml post as described here http://developer.yahoo.com/social/rest_api_guide/contacts-resource.html#contacts-xml_request_put
            uri = New Uri(System.Configuration.ConfigurationManager.AppSettings("YahooSocialUserUrl") & strUserID & "/contacts")
            objWebRequest = buildWebRequest(strAuthToken, strAuthTokenSecret, uri, "POST")

            'xml for adding contact with email. 
            postData = "<contact><fields><type>name</type><value><givenName>" & txtName.Text & "</givenName><middleName/><familyName></familyName><prefix/><suffix/><givenNameSound/><familyNameSound/></value></fields><fields><type>email</type><value>" & txtEmail.Text & "</value></fields></contact>"

            DATA = encoding.GetBytes(postData)
            objWebRequest.Method = "POST"
            objWebRequest.ContentLength = postData.Length

            'contenet type must be set to xml ig you are posting xml and not json
            objWebRequest.ContentType = "application/xml"
            stream = objWebRequest.GetRequestStream()
            stream.Write(DATA, 0, DATA.Length)
            stream.Close()

            'get the response from the server
            objResponse = objWebRequest.GetResponse

            objStreamReader = objResponse.GetResponseStream()
            objStreamResult = New System.IO.StreamReader(objStreamReader)
            strResult = objStreamResult.ReadToEnd

            objStreamReader.Close()
            objStreamResult.Close()
            objResponse.Close()

            'we are cool
            Response.Write("Contact has been added to your address book.")


        Catch ex As Exception
            Response.Write("Oh no.  Something bad happened. " & ex.ToString)
        End Try


    End Sub

    Function buildWebRequest(ByVal strAuthToken As String, ByVal strAuthTokenSecret As String, ByVal uri As Uri, ByVal strMethod As String) As System.Net.WebRequest
        'in order to call the social API we need to add the oauth headers to our outbound request.
        'note that the oauth signature method must match your actual http method (post, get) since each produces a different signature

        'Uses Oauth lib provided by Rob Richards (http://oauth.googlecode.com/svn/code/csharp/). Compiled into Assembly included in this project.
        Dim OAuthBase As New OAuth.OAuthBase
        Dim webRequest As System.Net.HttpWebRequest = CType(System.Net.WebRequest.Create(uri), HttpWebRequest)
        Dim strNonce As String = OAuthBase.GenerateNonce
        Dim strTimestamp As String = OAuthBase.GenerateTimeStamp
        Dim strConsumerKey As String = System.Configuration.ConfigurationManager.AppSettings("OAUTHConsumerKey")
        Dim strConsumerSecret As String = System.Configuration.ConfigurationManager.AppSettings("OAUTHConsumerSecret")

        'note that we have to do HMAC-SHA1 here since we are posting to http and not https.  
        Dim strSignature As String = OAuthBase.GenerateSignature(uri, strConsumerKey, strConsumerSecret, strAuthToken, strAuthTokenSecret, strMethod, strTimestamp, strNonce, "", "")

        webRequest.Headers.Add("Authorization", "OAuth oauth_nonce=" & Chr(34) & strNonce & Chr(34) & _
        ", oauth_timestamp=" & Chr(34) & strTimestamp & Chr(34) & _
        ", oauth_version=" & Chr(34) & "1.0" & Chr(34) & _
        ", oauth_signature_method=" & Chr(34) & "HMAC-SHA1" & Chr(34) & _
        ", oauth_consumer_key=" & Chr(34) & strConsumerKey & Chr(34) & _
        ", oauth_token=" & Chr(34) & strAuthToken & Chr(34) & _
        ", oauth_signature=" & Chr(34) & strSignature & Chr(34))


        Return webRequest
    End Function
End Class
