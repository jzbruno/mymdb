Imports System.ComponentModel
Imports System.Xml
Imports System.Drawing

Public Class Main

    ''' <summary>Activates the query to Amazon's web service upon click.</summary>
    Private Sub btnSearch_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSearch.Click
        PerformSearch()
    End Sub

    ''' <summary>Activates the query to Amazon's web service upon enter keypress.</summary>
    Private Sub txtKeywords_KeyUp(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles txtKeywords.KeyUp
        If e.KeyCode = Keys.Enter Then
            PerformSearch()
        End If
    End Sub

    ''' <summary>Performs the query of Amazon's web service.</summary>
    Private Sub PerformSearch()

        Dim searchResults As New BindingList(Of DVD)
        Dim amazonService As New SimpleAWSE

        Me.Cursor = Cursors.WaitCursor

        Try
            searchResults = amazonService.Search(Me.txtKeywords.Text, txtLocationS.Text)
            If searchResults IsNot Nothing Then
                Me.DVDBindingSource.DataSource = searchResults
            Else
                MsgBox("No Items Found.")
            End If
        Catch ex As Exception
            MsgBox(ex.Message)
        End Try

        Me.Cursor = Cursors.Default

        Me.lblResultCount.Text = searchResults.Count.ToString

    End Sub

    ''' <summary>Visits the selected DVD's Amazon page upon click.</summary>
    Private Sub btnVisitAmazon_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnVisitAmazon.Click
        If (Me.DVDBindingSource.Count > 0) AndAlso (Me.DVDBindingSource.Current IsNot Nothing) Then

            Dim currentDVD As DVD = CType(Me.DVDBindingSource.Current, DVD)
            Dim process As Process = New Process

            process.StartInfo.UseShellExecute = True
            process.StartInfo.FileName = currentDVD.URL
            process.Start()

        End If
    End Sub

    ''' <summary>Activate the add to collection sub.</summary>
    Private Sub btnAdd_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnAdd.Click
        AddToCollection()
    End Sub

    Private Sub AddToCollection()
        Try
            If (Me.DVDBindingSource.Count > 0) AndAlso (Me.DVDBindingSource.Current IsNot Nothing) Then

                Dim selected As DVD = CType(DVDBindingSource.Current, DVD)

                selected.Title = txtTitleS.Text
                selected.Plot = txtPlotS.Text
                selected.Year = txtYearS.Text
                selected.Length = txtLengthS.Text
                selected.Rated = txtRatedS.Text
                selected.Location = txtLocationS.Text
                selected.ASIN = selected.ASIN
                selected.URL = selected.URL

                picCoverS.Image.Save("covers/" & selected.Title & ".png", Imaging.ImageFormat.Png)
                selected.Image = "covers/" & selected.Title & ".png"

                CollectionBindingSource.Add(selected)
                SaveCollection(CollectionBindingSource.DataSource)

                MsgBox(selected.Title & " added to your collection.")

            Else
                MsgBox("No item selected. Select an item before adding.")
            End If
        Catch ex As Exception
            MsgBox(ex.Message)
        End Try
    End Sub

    Private Function AppendCollection(ByVal newDVD As DVD) As BindingList(Of DVD)
        Dim collection As BindingList(Of DVD) = LoadCollection()

        collection.Add(newDVD)
        SaveCollection(collection)

        Return collection
    End Function

    Private Function LoadCollection() As BindingList(Of DVD)
        Dim collection As New BindingList(Of DVD)
        Dim dvd As DVD

        Dim XMLDoc As New XmlDocument
        Dim XMLDocNodeList As XmlNodeList = XMLDoc.SelectNodes("dvd")
        Dim XMLDocNode As XmlNode

        Dim title As String = ""
        Dim plot As String = ""
        Dim imageurl As String = ""
        Dim asin As String = ""
        Dim url As String = ""
        Dim year As String = ""
        Dim rated As String = ""
        Dim length As String = ""
        Dim location As String = ""

        If Not My.Computer.FileSystem.FileExists("DVDCollection.xml") Then
            Dim wrtXML As XmlWriter = XmlWriter.Create("DVDCollection.xml")
            wrtXML.WriteStartElement("dvd")
            wrtXML.WriteEndElement()
            wrtXML.Flush()
            wrtXML.Close()
            MsgBox("No DVDCollection.xml file found, default created.")
        End If

        XMLDoc.Load("DVDCollection.xml")

        For Each XMLDocNode In XMLDocNodeList
            For i As Integer = 0 To XMLDocNode.ChildNodes.Count - 1
                Select Case XMLDocNode.ChildNodes(i).Name
                    Case "title"
                        title = XMLDocNode.ChildNodes(i).InnerText
                    Case "plot"
                        plot = XMLDocNode.ChildNodes(i).InnerText
                    Case "imageurl"
                        imageurl = XMLDocNode.ChildNodes(i).InnerText
                    Case "asin"
                        asin = XMLDocNode.ChildNodes(i).InnerText
                    Case "url"
                        url = XMLDocNode.ChildNodes(i).InnerText
                    Case "year"
                        year = XMLDocNode.ChildNodes(i).InnerText
                    Case "rated"
                        rated = XMLDocNode.ChildNodes(i).InnerText
                    Case "length"
                        length = XMLDocNode.ChildNodes(i).InnerText
                    Case "location"
                        location = XMLDocNode.ChildNodes(i).InnerText

                        dvd = New DVD( _
                        title, _
                        plot, _
                        imageurl, _
                        asin, _
                        url, _
                        year, _
                        rated, _
                        length, _
                        location)

                        collection.Add(dvd)
                End Select
            Next
        Next

        Return collection
    End Function

    Private Sub Main_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        ' make sure DVDCollection.xml exists if not create
        'If Not My.Computer.FileSystem.FileExists("DVDCollection.xml") Then
        '    Dim wrtXML As XmlWriter = XmlWriter.Create("DVDCollection.xml")
        '    wrtXML.WriteStartElement("dvd")
        '    wrtXML.WriteEndElement()
        '    wrtXML.Flush()
        '    MsgBox("No DVDCollection.xml file found, default created.")
        'End If

        ' make sure covers folder exists if not create
        'If Not My.Computer.FileSystem.DirectoryExists("covers/") Then
        '    My.Computer.FileSystem.CreateDirectory("covers/")
        'End If

        Dim collection As BindingList(Of DVD) = LoadCollection()

        If collection IsNot Nothing Then
            Me.CollectionBindingSource.DataSource = collection
        Else
            MsgBox("DVDCollection.xml is empty.")
        End If

        pnlAdd.Dock = DockStyle.Fill
        pnlBrowse.Dock = DockStyle.Fill

        pnlAdd.Visible = False
        pnlBrowse.Visible = True
        btnAddMode.Enabled = True
        btnBrowseMode.Enabled = False
    End Sub

    Private Sub btnAddMode_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnAddMode.Click
        pnlBrowse.Visible = False
        pnlAdd.Visible = True
        btnBrowseMode.Enabled = True
        btnAddMode.Enabled = False
    End Sub

    Private Sub btnBrowseMode_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnBrowseMode.Click
        pnlAdd.Visible = False
        pnlBrowse.Visible = True
        btnAddMode.Enabled = True
        btnBrowseMode.Enabled = False
    End Sub

    Private Sub btnLiveReset_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnLiveReset.Click

    End Sub

    Private Sub txtLiveKeywords_KeyUp(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles txtLiveKeywords.KeyUp

    End Sub

    Private Sub btnSave_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSave.Click
        UpdateCollection()
    End Sub

    Private Sub UpdateCollection()

        Dim selected As DVD = CType(CollectionBindingSource.Current, DVD)

        selected.Title = txtTitleC.Text
        selected.Plot = txtPlotC.Text
        selected.Year = txtYearC.Text
        selected.Length = txtLengthC.Text
        selected.Rated = txtRatedC.Text
        selected.Location = txtLocationC.Text
        selected.ASIN = selected.ASIN
        selected.URL = selected.URL

        If My.Computer.FileSystem.FileExists("covers/" & selected.Title & ".png") Then
            My.Computer.FileSystem.DeleteFile("covers/" & selected.Title & ".png")
        End If

        picCoverC.Image.Save("covers/" & selected.Title & ".png", Imaging.ImageFormat.Png)
        selected.Image = "covers/" & selected.Title & ".png"

        CollectionBindingSource.RemoveCurrent()
        CollectionBindingSource.Add(selected)
        SaveCollection(CollectionBindingSource.DataSource)

        MsgBox(selected.Title & " updated.")

    End Sub

    Private Sub btnRemove_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnRemove.Click

        If (Me.CollectionBindingSource.Count > 0) AndAlso (Me.CollectionBindingSource.Current IsNot Nothing) Then
            Dim selected As DVD = CType(CollectionBindingSource.Current, DVD)
            If My.Computer.FileSystem.FileExists("covers/" & selected.Title & ".png") Then
                My.Computer.FileSystem.DeleteFile("covers/" & selected.Title & ".png")
            End If

            CollectionBindingSource.RemoveCurrent()
            SaveCollection(CollectionBindingSource.DataSource)
            MsgBox("DVD removed from your collection.")

        End If

    End Sub

    Private Sub SaveCollection(ByVal collection As BindingList(Of DVD))
        Dim dvd As DVD
        Dim wrtXML As XmlWriter = XmlWriter.Create("DVDCollection.xml")
        wrtXML.WriteStartElement("dvd")

        For Each dvd In collection
            wrtXML.WriteElementString("title", dvd.Title)
            wrtXML.WriteElementString("plot", dvd.Plot)
            wrtXML.WriteElementString("imageurl", dvd.Image)
            wrtXML.WriteElementString("asin", dvd.ASIN)
            wrtXML.WriteElementString("url", dvd.URL)
            wrtXML.WriteElementString("year", dvd.Year)
            wrtXML.WriteElementString("rated", dvd.Rated)
            wrtXML.WriteElementString("length", dvd.Length)
            wrtXML.WriteElementString("location", dvd.Location)
        Next

        wrtXML.WriteEndElement()
        wrtXML.Flush()
        wrtXML.Close()
    End Sub

    Private Sub btnVisitAmazonC_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnVisitAmazonC.Click
        If (Me.CollectionBindingSource.Count > 0) AndAlso (Me.CollectionBindingSource.Current IsNot Nothing) Then

            Dim currentDVD As DVD = CType(Me.CollectionBindingSource.Current, DVD)
            Dim process As Process = New Process

            process.StartInfo.UseShellExecute = True
            process.StartInfo.FileName = currentDVD.URL
            process.Start()

        End If
    End Sub
End Class