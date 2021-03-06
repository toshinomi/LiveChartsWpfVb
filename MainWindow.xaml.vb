﻿Imports LiveCharts
Imports LiveCharts.Wpf
Imports System.Runtime.InteropServices.Marshal

Class MainWindow

    Private m_nHistgram(255) As Integer

    ''' <summary>
    ''' コンストラクタ
    ''' </summary>
    Public Sub New()

        ' この呼び出しはデザイナーで必要です。
        InitializeComponent()

        ' InitializeComponent() 呼び出しの後で初期化を追加します。

        InitGraph()

    End Sub

    ''' <summary>
    ''' ファイル選択ボタンのクリックイベント
    ''' </summary>
    ''' <param name="sender">オブジェクト</param>
    ''' <param name="e">ルーティングイベントのデータ</param>
    Private Sub OnClickBtnFileSelect(sender As Object, e As RoutedEventArgs)
        Dim openFileDlg As ComOpenFileDialog = New ComOpenFileDialog()
        openFileDlg.Filter = "JPG|*.jpg|PNG|*.png"
        openFileDlg.Title = "Open the file"
        If (openFileDlg.ShowDialog() = True) Then
            image.Source = Nothing

            Dim bitmap = New BitmapImage()
            bitmap.BeginInit()
            bitmap.UriSource = New Uri(openFileDlg.FileName)
            bitmap.EndInit()
            bitmap.Freeze()

            image.Source = bitmap

            DrawHistgram(bitmap)
        End If
        Return
    End Sub

    ''' <summary>
    ''' グラフ初期化
    ''' </summary>
    Public Sub InitGraph()
        Dim graphData = New GraphData()

        Dim chartValue = New ChartValues(Of Integer)()
        For nIdx As Integer = 0 To m_nHistgram.Length - 1
            m_nHistgram(nIdx) = 0
            chartValue.Add(m_nHistgram(nIdx))
        Next nIdx

        Dim SeriesCollection = New SeriesCollection()

        Dim lineSeriesChart = New LineSeries With
        {
            .Values = chartValue,
            .Title = "Histgram"
        }

        SeriesCollection.Add(lineSeriesChart)

        graphData.seriesCollection = SeriesCollection
        Me.DataContext = graphData

        Return
    End Sub

    ''' <summary>
    ''' グラフ描画
    ''' </summary>
    Public Sub DrawHistgram(_bitmap As BitmapImage)
        Dim GraphData = New GraphData()

        InitHistgram()

        CalHistgram(_bitmap)

        Dim chartValue = New ChartValues(Of Integer)()
        For nIdx As Integer = 0 To m_nHistgram.Length - 1
            chartValue.Add(m_nHistgram(nIdx))
        Next nIdx

        Dim SeriesCollection = New SeriesCollection()

        Dim lineSeriesChart = New LineSeries With
        {
            .Values = chartValue,
            .Title = "Histgram"
        }

        SeriesCollection.Add(lineSeriesChart)

        GraphData.seriesCollection = SeriesCollection
        Me.DataContext = GraphData
    End Sub

    ''' <summary>
    ''' イメージからヒストグラム用のデータ算出
    ''' </summary>
    Public Sub CalHistgram(_bitmap As BitmapImage)
        Dim nWidthSize As Integer = _bitmap.Width
        Dim nHeightSize As Integer = _bitmap.Height

        Dim wBitmap = New WriteableBitmap(_bitmap)

        Dim nIdxWidth As Integer
        Dim nIdxHeight As Integer

        For nIdxHeight = 0 To nHeightSize - 1 Step 1
            For nIdxWidth = 0 To nWidthSize - 1 Step 1
                Dim pAdr As IntPtr = wBitmap.BackBuffer
                Dim nPos As Integer = nIdxHeight * wBitmap.BackBufferStride + nIdxWidth * 4

                Dim nPixelB As Integer = ReadByte(pAdr, nPos + ComInfo.Pixel.B)
                Dim nPixelG As Integer = ReadByte(pAdr, nPos + ComInfo.Pixel.G)
                Dim nPixelR As Integer = ReadByte(pAdr, nPos + ComInfo.Pixel.R)

                Dim nGrayScale As Integer = (nPixelB + nPixelG + nPixelR) / 3

                m_nHistgram(nGrayScale) += 1
            Next
        Next
    End Sub

    ''' <summary>
    ''' ヒストグラム用のデータ初期化
    ''' </summary>
    Public Sub InitHistgram()
        For nIdx As Integer = 0 To m_nHistgram.Length - 1
            m_nHistgram(nIdx) = 0
        Next nIdx
    End Sub
End Class

''' <summary>
''' グラフデータのロジック
''' </summary>
Public Class GraphData
    Private m_seriesCollection As SeriesCollection

    ''' <summary>
    ''' シリーズコレクション
    ''' </summary>
    Public Property seriesCollection() As SeriesCollection
        Set(value As SeriesCollection)
            m_seriesCollection = value
        End Set
        Get
            Return m_seriesCollection
        End Get
    End Property
End Class