Imports System.IO

Public Class ArquivoRelatorio

    Public Shared Sub escreverCabecalho(ByVal diretorio As String, ByVal nomeArquivo As String, ByVal linha As Layout, ByVal ordemservico As String)
        Try
            Dim arquivo As StreamWriter
            If Not Directory.Exists(diretorio) Then
                Directory.CreateDirectory(diretorio)
            End If
            If File.Exists(diretorio & nomeArquivo) Then
                arquivo = New StreamWriter(diretorio & nomeArquivo, True)
            Else
                arquivo = New StreamWriter(diretorio & nomeArquivo)
            End If
            arquivo.WriteLine(Space(1) & "MARPRESS INFORMÁTICA")
            arquivo.WriteLine(Space(1) & "")
            arquivo.WriteLine(Space(1) & "RELATÓRIO DE PRODUÇÃO - FATURAS Lopes FAC " & linha.CIF.TipoRegistro)
            arquivo.WriteLine(Space(1) & "MODELO " & linha.CIF.TipoRegistro)
            arquivo.WriteLine(Space(1) & "")
            arquivo.WriteLine(Space(1) & "Nome do Cliente : " & linha.Remetente.Apelido)
            arquivo.WriteLine(Space(1) & "Nome do Arquivo : " & nomeArquivo)
            arquivo.WriteLine(Space(1) & "Data do Processamento : " & FormatDateTime(Date.Now, DateFormat.ShortDate))
            'arquivo.WriteLine(Space(1) & "Data de Postagem : " & linha.Boleto.DataProcessamento)
            arquivo.WriteLine(Space(1) & "Ordem de Serviço (OS) : " & ordemservico)
            arquivo.WriteLine(Space(1) & "")
            arquivo.WriteLine(Space(1) & "N_CONTA" & Space(4) & ";" & "N_CARTAO" & Space(11) & ";" & "NOME" & Space(36) & ";" & "VENCIMENTO" & Space(0) & ";" & "CODIGO CIF")
            arquivo.WriteLine(Space(1) & "")
            arquivo.Flush()
            arquivo.Close()
        Catch ex As Exception
            Throw New Exception(ex.Message)
        End Try
    End Sub

    Public Shared Sub escreverDetalhe(ByVal diretorio As String, ByVal nomeArquivo As String, ByVal linha As Layout, ByVal sequencia As Integer)
        Try
            Dim arquivo As StreamWriter
            If Not Directory.Exists(diretorio) Then
                Directory.CreateDirectory(diretorio)
            End If
            If File.Exists(diretorio & nomeArquivo) Then
                arquivo = New StreamWriter(diretorio & nomeArquivo, True)
            Else
                arquivo = New StreamWriter(diretorio & nomeArquivo)
            End If
            arquivo.WriteLine(linha.Destinatario.Documento.Trim.PadRight(12) & ";" & linha.Cartoes(0).Numero.PadRight(19) & ";" & linha.Destinatario.Nome.PadRight(40) & ";" & linha.Boleto.Parcelas(0).Vencimento.Trim.PadRight(10) & ";" & linha.CIF.CodigoCIF)
            arquivo.Flush()
            arquivo.Close()
        Catch ex As Exception
            Throw New Exception(ex.Message)
        End Try
    End Sub

    Public Shared Sub escreverFim(ByVal diretorio As String, ByVal nomeArquivo As String, ByVal local As Integer, ByVal estadual As Integer, ByVal nacional As Integer)
        Try
            Dim arquivo As StreamWriter
            If Not Directory.Exists(diretorio) Then
                Directory.CreateDirectory(diretorio)
            End If
            If File.Exists(diretorio & nomeArquivo) Then
                arquivo = New StreamWriter(diretorio & nomeArquivo, True)
            Else
                arquivo = New StreamWriter(diretorio & nomeArquivo)
            End If
            arquivo.WriteLine(New String("-", 261))
            arquivo.WriteLine("QUANTITATIVO")
            arquivo.WriteLine("       Local : " + FormatNumber(local, 0))
            arquivo.WriteLine("    Estadual : " + FormatNumber(estadual, 0))
            arquivo.WriteLine("    Nacional : " + FormatNumber(nacional, 0))
            arquivo.WriteLine(New String("-", 261))
            arquivo.Flush()
            arquivo.Close()
        Catch ex As Exception
            Throw New Exception(ex.Message)
        End Try
    End Sub

End Class
