Imports System.Net
Imports System.Web.HttpUtility
Imports System.Text.RegularExpressions

Partial Public Class _Default
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        Try

        
            Dim OAuthBase As New OAuth.OAuthBase
            Dim objStreamReader As System.IO.StreamReader = Nothing
            Dim objStream As System.IO.Stream = Nothing
            Dim strResult As String
            Dim strTokenSecret As String
            Dim strToken As String


            'step 1 in Oauth is to get a request token. The OAUTHConsumerKey and OAUTHConsumerSecret must be your own.  If you don't already have one, 
            'go to  https://developer.apps.yahoo.com/dashboard/createKey.html and sign up for one. 
            'Then update the web config with your OAUTHConsumerKey and OAUTHConsumerSecret
            'Also, it is really important that you specify the correct application url when signing up.  Yahoo! will not allow oauth_callback to a domain that is different from your application url domain.
            'Make sure you update the OAUTHCallbackURL to whatever domain you created your key with plus /auth.aspx (e.g., http://my.domain.com/auth.aspx)
            'Also, please be sure that when you set up your key that you ask for read/write access to Yahoo! Mail and Yahoo! Contacts.  If you don't then your credentials
            'won't work for this example.

            Dim strRequest As String = System.Configuration.ConfigurationManager.AppSettings("OAUTHGetRequestTokenURL") & _
            "oauth_nonce=" & OAuthBase.GenerateNonce & _
            "&oauth_timestamp=" & OAuthBase.GenerateTimeStamp & _
            "&oauth_consumer_key=" & System.Configuration.ConfigurationManager.AppSettings("OAUTHConsumerKey") & _
            "&oauth_signature_method=plaintext" & _
            "&oauth_signature=" & System.Configuration.ConfigurationManager.AppSettings("OAUTHConsumerSecret") & "%26" & _
            "&oauth_version=1.0" & _
            "&oauth_callback=" & UrlEncode(System.Configuration.ConfigurationManager.AppSettings("OAUTHCallbackURL"))  'this variable indcates the url that the user will be redirected to after authenticating

            'all we are doing here is creating an http get and passing in our oauth variables to get a request token
            Dim objHTTPRequest As HttpWebRequest
            Dim objHTTPResponse As HttpWebResponse

            objHTTPRequest = CType(WebRequest.Create(strRequest), HttpWebRequest)
            objHTTPResponse = CType(objHTTPRequest.GetResponse(), HttpWebResponse)
            objStream = objHTTPResponse.GetResponseStream()
            objStreamReader = New System.IO.StreamReader(objStream)

            'read the results from our http get
            strResult = objStreamReader.ReadToEnd

            'use regex to get our token and secret for the request token
            strToken = getToken(strResult)
            strTokenSecret = getTokenSecret(strResult)

            Session("oauth_token_secret") = strTokenSecret

            objStreamReader.Close()
            objStream.Close()
            objHTTPResponse.Close()

            'now we actually redirect the user to the auth token url.  This is the page hosted by yahoo that will ask the user to enter their password and confirm they want to link their account.
            strRequest = System.Configuration.ConfigurationManager.AppSettings("OAUTHGetRequestAuthURL") & "oauth_token=" & strToken
            Response.Redirect(strRequest)
        Catch ex As Exception
            Response.Write("Something bad happened while trying to get a request token. " & ex.ToString)

        End Try
    End Sub
    Function getToken(ByVal strResult As String) As String
        Try

            Dim myMatch As Match = System.Text.RegularExpressions.Regex.Match(strResult, "oauth_token=([A-Z0-9]{1,20})", RegexOptions.IgnoreCase)
            Return myMatch.Groups(1).Value.ToString

        Catch ex As Exception
            Return ""
        End Try


    End Function
    Function getTokenSecret(ByVal strResult As String) As String
        Try

            Dim myMatch As Match = System.Text.RegularExpressions.Regex.Match(strResult, "oauth_token_secret=([A-Z0-9]{1,100})", RegexOptions.IgnoreCase)
            Return myMatch.Groups(1).Value.ToString

        Catch ex As Exception
            Return ""
        End Try


    End Function
End Class
