Imports System.IO
Imports Marpress.Funcoes
Imports Marpress.FAC
Imports Marpress.Interfaces

Public Class Processamento

    Shared Sub processa(ByVal diretorio As String, ByVal arquivoEntrada As String, ByVal tipofac As Tipo, ByVal contratofac As Contrato, ByVal label As Label, ByVal dataFAC As Date, ByVal producao As Boolean, ByVal ordemdeserviço As String)
        Try
            Dim dia As String = String.Format("{0:ddMMyyyy}", Now.Date)
            Dim nomearquivo As String = Left(arquivoEntrada, arquivoEntrada.Length - 4)

            label.Text = "Lendo o arquivo..."
            Dim arquivo As New ArquivoFatura(diretorio & arquivoEntrada, label)
            Dim cont As Integer = 0
            Dim sequencia_a4 As Integer = 0
            Dim sequencia_a3c As Integer = 0
            Dim sequencia_a3n As Integer = 0
            Dim respa4 As Boolean = False
            Dim respa3n As Boolean = False
            Dim respa3c As Boolean = False
            Dim total As Integer = 0
            Dim lote As Integer = 0
            Dim imagemfrente As String = ""
            Dim imagemverso As String = ""

            processaFAC(contratofac, "A4", arquivo, producao, dataFAC, label)
            processaFAC(contratofac, "A3C", arquivo, producao, dataFAC, label)
            processaFAC(contratofac, "A3N", arquivo, producao, dataFAC, label)

            File.Delete(diretorio & nomearquivo & "_" & arquivo.Linhas(0).CIF.TipoRegistro & ".sem")
            File.Delete(diretorio & nomearquivo & "_99999" & ".dev")
            File.Delete(diretorio & nomearquivo & arquivo.Linhas(0).CIF.TipoRegistro & ".err")
            File.Delete(diretorio & nomearquivo & "_A4.rel")
            File.Delete(diretorio & nomearquivo & "_A3C.rel")
            File.Delete(diretorio & nomearquivo & "_A3N.rel")
            File.Delete(diretorio & nomearquivo & "_A4.os")
            File.Delete(diretorio & nomearquivo & "_A3C.os")
            File.Delete(diretorio & nomearquivo & "_A3N.os")

            Dim local_a4 As Integer = 0
            Dim estadual_a4 As Integer = 0
            Dim nacional_a4 As Integer = 0

            Dim local_a3c As Integer = 0
            Dim estadual_a3c As Integer = 0
            Dim nacional_a3c As Integer = 0

            Dim local_a3n As Integer = 0
            Dim estadual_a3n As Integer = 0
            Dim nacional_a3n As Integer = 0

            'Dim selecao As IEnumerable(Of ICIF) = arquivo.Linhas.OrderBy(Function(arqNot) arqNot.TipoRegistro)

            'arquivo.Linhas = selecao.ToList

            label.Text += vbCrLf & "Processando..."
            Dim texto As String = label.Text & vbCrLf
            For Each linha As Layout In arquivo.Linhas
                total += 1
                If linha.CIF.TipoRegistro = "CepErrado" Then
                    ArquivoSaida.escrever(diretorio, nomearquivo & ".err", linha.MontarLinha)
                ElseIf linha.CIF.TipoRegistro = "SemCodigoDeBarras  A4" Then
                    ArquivoSaida.escrever(diretorio, nomearquivo & ".sem", linha.MontarLinha)
                ElseIf linha.CIF.TipoRegistro = "SemCodigoDeBarras  A3C" Then
                    ArquivoSaida.escrever(diretorio, nomearquivo & ".sem", linha.MontarLinha)
                ElseIf linha.CIF.TipoRegistro = "SemCodigoDeBarras  A3N" Then
                    ArquivoSaida.escrever(diretorio, nomearquivo & ".sem", linha.MontarLinha)
                Else
                    If linha.CIF.TipoRegistro = "A4" Then
                        If sequencia_a4 = 0 And Not respa4 Then
                            respa4 = True
                            ArquivoRelatorio.escreverCabecalho(diretorio, nomearquivo & "_" & linha.CIF.TipoRegistro & ".REL", linha, ordemdeserviço)
                        End If
                    End If
                    If linha.CIF.TipoRegistro = "A3C" Then
                        If sequencia_a3c = 0 And Not respa3c Then
                            respa3c = True
                            ArquivoRelatorio.escreverCabecalho(diretorio, nomearquivo & "_" & linha.CIF.TipoRegistro & ".REL", linha, ordemdeserviço)
                        End If
                    End If
                    If linha.CIF.TipoRegistro = "A3N" Then
                        If sequencia_a3n = 0 And Not respa3n Then
                            respa3n = True
                            ArquivoRelatorio.escreverCabecalho(diretorio, nomearquivo & "_" & linha.CIF.TipoRegistro & ".REL", linha, ordemdeserviço)
                        End If
                    End If
                    If linha.CIF.TipoRegistro = "A4" Then
                        sequencia_a4 += 1
                        ArquivoRelatorio.escreverDetalhe(diretorio, nomearquivo & "_" & linha.CIF.TipoRegistro & ".REL", linha, sequencia_a4)
                    End If
                    If linha.CIF.TipoRegistro = "A3C" Then
                        sequencia_a3c += 1
                        ArquivoRelatorio.escreverDetalhe(diretorio, nomearquivo & "_" & linha.CIF.TipoRegistro & ".REL", linha, sequencia_a3c)
                    End If
                    If linha.CIF.TipoRegistro = "A3N" Then
                        sequencia_a3n += 1
                        ArquivoRelatorio.escreverDetalhe(diretorio, nomearquivo & "_" & linha.CIF.TipoRegistro & ".REL", linha, sequencia_a3n)
                    End If
                    lote = linha.CIF.CodigoCIF.Substring(10, 5)
                    cont += 1
                    Application.DoEvents()
                    Devolucao.Criar(diretorio, nomearquivo & "_" & lote & ".DEV", linha)
                    linha.IDProcessamento = cont.ToString.PadLeft(7, "0")
                    linha.ModeloFatura = New ModeloFatura(DirectCast(linha, Layout))
                    If linha.CIF.TipoRegistro = "A4" Then
                        If SomenteNumeros(linha.Destinatario.Endereco.CEP).PadLeft(8, "0").Substring(0, 1) = "0" Then
                            local_a4 += 1
                        ElseIf SomenteNumeros(linha.Destinatario.Endereco.CEP).PadLeft(8, "0").Substring(0, 1) = "1" Then
                            estadual_a4 += 1
                        Else
                            nacional_a4 += 1
                        End If
                    End If
                    If linha.CIF.TipoRegistro = "A3C" Then
                        If SomenteNumeros(linha.Destinatario.Endereco.CEP).PadLeft(8, "0").Substring(0, 1) = "0" Then
                            local_a3c += 1
                        ElseIf SomenteNumeros(linha.Destinatario.Endereco.CEP).PadLeft(8, "0").Substring(0, 1) = "1" Then
                            estadual_a3c += 1
                        Else
                            nacional_a3c += 1
                        End If
                    End If
                    If linha.CIF.TipoRegistro = "A3N" Then
                        If SomenteNumeros(linha.Destinatario.Endereco.CEP).PadLeft(8, "0").Substring(0, 1) = "0" Then
                            local_a3n += 1
                        ElseIf SomenteNumeros(linha.Destinatario.Endereco.CEP).PadLeft(8, "0").Substring(0, 1) = "1" Then
                            estadual_a3n += 1
                        Else
                            nacional_a3n += 1
                        End If
                    End If
                    label.Text = texto & "Definindo os modelos ... " & cont
                End If
            Next
            If sequencia_a4 > 0 Then
                ArquivoRelatorio.escreverFim(diretorio, nomearquivo & "_" & "A4.REL", local_a4, estadual_a4, nacional_a4)
            End If
            If sequencia_a3c > 0 Then
                ArquivoRelatorio.escreverFim(diretorio, nomearquivo & "_" & "A3C.REL", local_a3c, estadual_a3c, nacional_a3c)
            End If
            If sequencia_a3n > 0 Then
                ArquivoRelatorio.escreverFim(diretorio, nomearquivo & "_" & "A3N.REL", local_a3n, estadual_a3n, nacional_a3n)
            End If

            Dim selecao As IEnumerable(Of ICIF) = From linhas In arquivo.Linhas Where linhas.CIF.TipoRegistro <> "CepErrado" And linhas.CIF.TipoRegistro <> "SemCodigoDeBarras" Order By linhas.CIF.TipoRegistro, linhas.CIF.CodigoCIF

            arquivo.Linhas = selecao.ToList
            ArquivoPDF.criar(diretorio, nomearquivo, arquivo, imagemfrente, imagemverso, label)

            If cont > 0 Then
                If producao Then
                    File.Move(diretorio & nomearquivo & "_" & lote & ".DEV", "U:\ArqDev\" & nomearquivo & "_" & lote & ".DEV")
                End If
            End If

            label.Text &= vbCrLf & "Arquivo Processado!!!"
            MessageBox.Show("Arquivo Processado!!!", "", MessageBoxButtons.OK, MessageBoxIcon.Information)
        Catch ex As Exception
            MessageBox.Show(ex.Message, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

End Class
