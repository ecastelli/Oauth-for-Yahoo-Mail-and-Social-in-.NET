Imports System.Net
Imports System.Web.HttpUtility
Imports System.Text.RegularExpressions

Partial Public Class auth
    Inherits System.Web.UI.Page

    Private Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Try

        
            Dim OAuthBase As New OAuth.OAuthBase
            Dim objStreamReader As System.IO.StreamReader = Nothing
            Dim objStream As System.IO.Stream = Nothing
            Dim strResult As String

            'if we reach this page then the consumer has entered all the required info on https://api.login.yahoo.com/oauth/v2/request_auth to 
            'link their account with your app.  Yahoo! returns the oauth_token and oauth_verifier in the query string back to this page.  
            'we need now need to use this to get a token for the user
            Dim strRequest As String = System.Configuration.ConfigurationManager.AppSettings("OAUTHGetRequestConsumerToken") & _
            "oauth_nonce=" & OAuthBase.GenerateNonce & _
            "&oauth_timestamp=" & OAuthBase.GenerateTimeStamp & _
            "&oauth_consumer_key=" & System.Configuration.ConfigurationManager.AppSettings("OAUTHConsumerKey") & _
            "&oauth_signature_method=plaintext" & _
            "&oauth_signature=" & System.Configuration.ConfigurationManager.AppSettings("OAUTHConsumerSecret") & "%26" & Session("oauth_token_secret") & _
            "&oauth_version=1.0" & _
            "&oauth_token=" & Request("oauth_token") & _
            "&oauth_verifier=" & Request("oauth_verifier")

            'now we are doing yet another http get. this time we are going to make a call to geth the consumer token.
            Dim objHTTPRequest As HttpWebRequest
            Dim objHTTPResponse As HttpWebResponse

            objHTTPRequest = CType(WebRequest.Create(strRequest), HttpWebRequest)

            objHTTPResponse = CType(objHTTPRequest.GetResponse(), HttpWebResponse)
            objStream = objHTTPResponse.GetResponseStream()
            objStreamReader = New System.IO.StreamReader(objStream)


            strResult = objStreamReader.ReadToEnd

            objStreamReader.Close()
            objStream.Close()
            objHTTPResponse.Close()

            'use regex to get the consumer token and secret from our http response and store these in session variables for future calls.
            Session("oauth_token") = getAuthToken(strResult)
            Session("oauth_token_secret") = getAuthTokenSecret(strResult)
            Response.Redirect("PostAuthSamples.aspx")
        Catch ex As Exception
            Response.Write("Something bad happended trying to get a consumer token " & ex.ToString)
        End Try

    End Sub

    Function getAuthToken(ByVal strResult As String) As String
        Try

            Dim myMatch As Match = System.Text.RegularExpressions.Regex.Match(strResult, "oauth_token=([^&]{1,})", RegexOptions.IgnoreCase)
            Return myMatch.Groups(1).Value.ToString

        Catch ex As Exception
            Return ""
        End Try


    End Function
    Function getAuthTokenSecret(ByVal strResult As String) As String
        Try

            Dim myMatch As Match = System.Text.RegularExpressions.Regex.Match(strResult, "oauth_token_secret=([A-Z0-9]{1,100})", RegexOptions.IgnoreCase)
            Return myMatch.Groups(1).Value.ToString

        Catch ex As Exception
            Return ""
        End Try


    End Function
End Class