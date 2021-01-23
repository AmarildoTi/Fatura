Imports Marpress.Interfaces
Imports System.IO
Imports System.Reflection
Imports Marpress.Padrao

Public Class ArquivoFatura
    Implements IArquivo

    Private arquivo As StreamReader

    Private _linhas As List(Of ICIF)
    Public Property Linhas() As List(Of ICIF) Implements IArquivo.Linhas
        Get
            Return _linhas
        End Get
        Set(ByVal value As List(Of ICIF))
            _linhas = value
        End Set
    End Property

    Public Sub New(ByVal nomeArquivoEntrada As String, ByVal label As Label)
        Try
            arquivo = New StreamReader(nomeArquivoEntrada, System.Text.Encoding.Default)
            Dim linhas As New List(Of ICIF)
            linhas = lerArquivo(label)
            _linhas = linhas
        Catch ex As Exception
            Throw New Exception(ex.Message)
        End Try
    End Sub

    Private Function lerArquivo(ByVal label As Label) As List(Of ICIF)
        Dim linhasArquivo As New List(Of ICIF)
        Try
            Dim fatura As New Layout
            Dim i As Integer = 0
            Dim devedor As Integer = 0
            Dim texto As String = label.Text
            Dim tipo0 As String = arquivo.ReadLine
            If tipo0.Substring(0, 2) <> 0 Then
                Throw New Exception("Arquivo Sem registro 00")
            End If
            While arquivo.Peek > -1
                Dim linha As String = arquivo.ReadLine
                If linha.Substring(0, 2) = "01" Then
                    i += 1
                    If i > 1 Then
                        If fatura.Cartoes(0).Despesa.Count <= 50 Then
                            fatura.CIF.TipoRegistro = "A4"
                        ElseIf fatura.Cartoes(0).Despesa.Count > 50 Then
                            fatura.CIF.TipoRegistro = "A3N"
                        End If
                        If Not fatura.QtdeParcela Is Nothing Then
                            fatura.CIF.TipoRegistro = "A3C"
                        End If
                        linhasArquivo.Add(fatura)
                        fatura = New Layout
                    End If
                    fatura.CarregaRegistros(tipo0)
                End If
                fatura.CarregaRegistros(linha)
                label.Text = texto & i
            End While

            If fatura.Cartoes(0).Despesa.Count <= 50 Then
                fatura.CIF.TipoRegistro = "A4"
            ElseIf fatura.Cartoes(0).Despesa.Count > 50 Then
                fatura.CIF.TipoRegistro = "A3N"
            End If
            If Not fatura.QtdeParcela Is Nothing Then
                fatura.CIF.TipoRegistro = "A3C"
            End If
            linhasArquivo.Add(fatura)

        Catch ex As Exception
            MessageBox.Show(ex.Message, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try

        Return linhasArquivo

    End Function

End Class
