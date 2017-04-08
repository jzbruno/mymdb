Imports System.Collections.Generic
Imports System.ComponentModel
Imports System.Text.RegularExpressions
Imports MyMDB.AmazonWS

Public Class SimpleAWSE

    Public Function Search(ByVal keywords As String, ByVal userLocation As String) As BindingList(Of DVD)

        Dim searchResults As BindingList(Of DVD)
        Dim aws As New AWSECommerceService

        Dim itemSearchRequest As New ItemSearchRequest
        Dim itemSearch As New ItemSearch

        Dim amazonResponse As ItemSearchResponse = Nothing
        Dim amazonItems As Item() = Nothing

        With itemSearchRequest
            .Keywords = keywords
            .SearchIndex = "DVD"
            .ResponseGroup = New String() {"Medium"}
            .Sort = "salesrank"
        End With

        With itemSearch
            .AssociateTag = "jzbruno-20"
            .AWSAccessKeyId = "15JS1RYSCY6YQNFANP02"
            .Request = New ItemSearchRequest() {itemSearchRequest}
        End With

        amazonResponse = aws.ItemSearch(itemSearch)

        If amazonResponse IsNot Nothing Then
            amazonItems = amazonResponse.Items(0).Item
        End If

        searchResults = GetListOfDVDs(amazonItems, userLocation)

        Return searchResults

    End Function

    Private Function GetListOfDVDs(ByVal amazonItems As Item(), ByVal userLocation As String) As BindingList(Of DVD)

        Dim dvds As New BindingList(Of DVD)
        Dim dvd As DVD

        If amazonItems IsNot Nothing Then

            For Each amazonItem As Item In amazonItems

                Dim title As String = ""
                If amazonItem.ItemAttributes.Title IsNot Nothing Then
                    title = amazonItem.ItemAttributes.Title
                End If
                Dim rated As String = ""
                If amazonItem.ItemAttributes.AudienceRating IsNot Nothing Then
                    rated = amazonItem.ItemAttributes.AudienceRating
                End If

                dvd = New DVD( _
                title, _
                Me.ReviewToString(amazonItem), _
                Me.ImageURLToString(amazonItem), _
                Me.ASINToString(amazonItem), _
                Me.URLToString(amazonItem), _
                Me.DateToString(amazonItem), _
                rated, _
                Me.RunningTimeToString(amazonItem), _
                userLocation)

                dvds.Add(dvd)
            Next

        End If

        Return dvds

    End Function

    Private Function ReviewToString(ByVal amazonItem As Item) As String
        Dim review As String = ""
        Dim reviews As EditorialReview() = amazonItem.EditorialReviews

        If (reviews IsNot Nothing) AndAlso (reviews.Length > 0) Then
            review = Me.StripTags(reviews(0).Content)
        Else
            review = "No description found."
        End If

        Return review
    End Function

    Private Function StripTags(ByVal text As String) As String
        If text IsNot Nothing Then
            'remove HTML tags
            text = Regex.Replace(text, "(<[^>]+>)", "")
            'remove spaces
            text = Regex.Replace(text, "&(nbsp|#160);", "")
        End If

        Return text
    End Function

    Private Function URLToString(ByVal amazonItem As Item) As String
        Dim url As String = ""

        If amazonItem.DetailPageURL IsNot Nothing Then
            url = amazonItem.DetailPageURL
        End If

        Return url
    End Function

    Private Function ImageURLToString(ByVal amazonItem As Item) As String
        Dim url As String = ""

        If amazonItem.MediumImage.URL IsNot Nothing Then
            url = amazonItem.MediumImage.URL
        End If

        Return url
    End Function

    Private Function ASINToString(ByVal amazonItem As Item) As String
        Dim asin As String = ""

        If (amazonItem IsNot Nothing) AndAlso (amazonItem.ASIN.Length > 0) Then
            asin = amazonItem.ASIN.ToString
        End If

        Return asin
    End Function

    Private Function DateToString(ByVal amazonItem As Item) As String
        Dim year As String = ""

        If amazonItem.ItemAttributes.ReleaseDate IsNot Nothing Then
            year = Regex.Split(amazonItem.ItemAttributes.ReleaseDate, "(-)")(0)
        End If

        Return year
    End Function

    Private Function RunningTimeToString(ByVal amazonItem As Item) As String
        Dim length As String = ""

        If amazonItem.ItemAttributes.RunningTime IsNot Nothing Then
            length = amazonItem.ItemAttributes.RunningTime.Value
        End If

        Return length
    End Function

End Class