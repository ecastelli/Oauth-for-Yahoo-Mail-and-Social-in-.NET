Imports Microsoft.VisualBasic

Public Class clsMailFolder
    Private mstrFolderName As String
    Private mstrFolderID As String
    Private mintMailQTY As Integer
    Private mintRequestFolderID As Integer
    Private mintProcessedQTY As Integer

    Public Property FolderName() As String
        Get
            Return mstrFolderName
        End Get
        Set(ByVal value As String)
            mstrFolderName = value
        End Set
    End Property
    Public Property FolderID() As String
        Get
            Return mstrFolderID
        End Get
        Set(ByVal value As String)
            mstrFolderID = value
        End Set
    End Property
    Public Property RequestFolderID() As Integer
        Get
            Return mintRequestFolderID
        End Get
        Set(ByVal value As Integer)
            mintRequestFolderID = value
        End Set
    End Property


    Public Property MailQTY() As Integer
        Get
            Return mintMailQTY
        End Get
        Set(ByVal value As Integer)
            mintMailQTY = value
        End Set
    End Property

End Class
