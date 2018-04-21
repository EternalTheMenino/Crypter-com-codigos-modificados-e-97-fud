Imports System
Imports System.Text
Imports System.Threading

Public Class ProgressBar
    Implements IDisposable
    Implements IProgress(Of Double)

    Private ReadOnly _animationInterval As TimeSpan = TimeSpan.FromSeconds(1.0 / 8)
    Private Const _animation = "|/-\"
    Private _animationIndex As Integer = 0

    Private _currentProgress As Double = 0.0
    Private _currentText As String = String.Empty

    Public Sub Report(value As Double) Implements IProgress(Of Double).Report
        ' Make sure value is in [0..1] range
        value = Math.Max(0, Math.Min(1, value))
        Interlocked.Exchange(_currentProgress, value)
    End Sub

    Public Sub UpdateText(text As String)
        ' Get length of common portion
        Dim commonPrefixLength = 0
        Dim commonLength = Math.Min(_currentText.Length, text.Length)
        While commonPrefixLength < commonLength _
            AndAlso text(commonPrefixLength) = _currentText(commonPrefixLength)
            commonPrefixLength += 1
        End While

        ' Backtrack to the first differing character
        Dim outputBuilder = New StringBuilder()
        outputBuilder.Append(ControlChars.Back, _currentText.Length - commonPrefixLength)

        ' Output new suffix
        outputBuilder.Append(text.Substring(commonPrefixLength))

        ' If the new text is shorter than the old one: delete overlapping characters
        Dim overlapCount = _currentText.Length - text.Length

        If overlapCount > 0 Then
            outputBuilder.Append(" "c, overlapCount)
            outputBuilder.Append(ControlChars.Back, overlapCount)
        End If

        Console.Write(outputBuilder.ToString())
        _currentText = text
    End Sub

    Private Const _blockCount = 10
    Private _disposed As Boolean = False
    Private ReadOnly _timer As Timer

    Public Sub ResetTimer()
        _timer.Change(_animationInterval, TimeSpan.FromMilliseconds(-1))
    End Sub

    Private Sub TimerHandler(state As Object)
        SyncLock _timer
            If _disposed Then Return

            Dim progressBlockCount = Convert.ToInt32(_currentProgress * _blockCount)
            Dim percent = Convert.ToInt32(_currentProgress * 100.0)
            Dim text = String.Format(
                "[{0}{1}] {2,3}% {3}",
                New String("#"c, progressBlockCount),
                New String("-"c, _blockCount - progressBlockCount),
                percent,
                _animation(Interlocked.Increment(_animationIndex) Mod _animation.Length)
                )
            UpdateText(text)

            ResetTimer()
        End SyncLock
    End Sub

    Public Sub Dispose() Implements IDisposable.Dispose
        SyncLock _timer
            _disposed = True
            UpdateText(String.Empty)
        End SyncLock
    End Sub

    Public Sub New()
        _timer = New Timer(AddressOf TimerHandler)

        ' A progress bar is only for temporary display in a console window.
        ' If the console output is redirected to a file, draw nothing.
        ' Otherwise, we'll end up with a lot of garbage in the target file.
        If Not Console.IsOutputRedirected Then
            ResetTimer()
        End If
    End Sub
End Class