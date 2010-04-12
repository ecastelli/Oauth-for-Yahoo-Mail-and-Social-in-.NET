Imports Microsoft.VisualBasic
Imports System.Diagnostics
Imports System.Web.Services
Imports System.ComponentModel
Imports System.Web.Services.Protocols
Imports System
Imports System.Xml.Serialization
Imports System.Web


Partial Public Class YAPI

    Inherits Ymail.ymws
    Public strAuthToken As String
    Public strAuthTokenSecret As String
    Protected Overloads Overrides Function GetWebRequest(ByVal uri As Uri) As System.Net.WebRequest
        'Adds our OAUTH authentication headers.
        'Uses Oauth lib provided by Rob Richards (http://oauth.googlecode.com/svn/code/csharp/). Compiled into Assembly included in this project.

        Dim webRequest As System.Net.HttpWebRequest = DirectCast(MyBase.GetWebRequest(uri), System.Net.HttpWebRequest)
        Dim OAuthBase As New OAuth.OAuthBase
        Dim strNonce As String = OAuthBase.GenerateNonce
        Dim strTimestamp As String = OAuthBase.GenerateTimeStamp
        Dim strConsumerKey As String = System.Configuration.ConfigurationManager.AppSettings("OAUTHConsumerKey")
        Dim strConsumerSecret As String = System.Configuration.ConfigurationManager.AppSettings("OAUTHConsumerSecret")

        Dim strSignature As String = OAuthBase.GenerateSignature(uri, strConsumerKey, strConsumerSecret, strAuthToken, strAuthTokenSecret, "POST", strTimestamp, strNonce, "", "")

        'note that we have to do HMAC-SHA1 here since we are posting to http and not https.  
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
