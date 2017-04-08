Imports System.Collections.Generic

<Serializable()> Public Class DVD

    Private m_Title As String = ""
    Private m_Plot As String = ""
    Private m_Image As String = ""
    Private m_ASIN As String = ""
    Private m_URL As String = ""
    Private m_Year As String = ""
    Private m_Rated As String = ""
    Private m_Length As String = ""
    Private m_Location As String = ""

    Public Sub New(ByVal title As String, ByVal plot As String, ByVal image As String, ByVal asin As String, ByVal url As String, ByVal year As String, ByVal rated As String, ByVal length As String, ByVal location As String)

        m_Title = title
        m_Plot = plot
        m_Image = image
        m_ASIN = asin
        m_URL = url
        m_Year = year
        m_Rated = rated
        m_Length = length
        m_Location = location

    End Sub

    Public Property Title() As String
        Get
            Return m_Title
        End Get
        Set(ByVal value As String)
            m_Title = value
        End Set
    End Property

    Public Property Plot() As String
        Get
            Return m_Plot
        End Get
        Set(ByVal value As String)
            m_Plot = value
        End Set
    End Property

    Public Property Image() As String
        Get
            Return m_Image
        End Get
        Set(ByVal value As String)
            m_Image = value
        End Set
    End Property

    Public Property ASIN() As String
        Get
            Return m_ASIN
        End Get
        Set(ByVal value As String)
            m_ASIN = value
        End Set
    End Property

    Public Property URL() As String
        Get
            Return m_URL
        End Get
        Set(ByVal value As String)
            m_URL = value
        End Set
    End Property

    Public Property Year() As String
        Get
            Return m_Year
        End Get
        Set(ByVal value As String)
            m_Year = value
        End Set
    End Property

    Public Property Rated() As String
        Get
            Return m_Rated
        End Get
        Set(ByVal value As String)
            m_Rated = value
        End Set
    End Property

    Public Property Length() As String
        Get
            Return m_Length
        End Get
        Set(ByVal value As String)
            m_Length = value
        End Set
    End Property

    Public Property Location() As String
        Get
            Return m_Location
        End Get
        Set(ByVal value As String)
            m_Location = value
        End Set
    End Property

End Class