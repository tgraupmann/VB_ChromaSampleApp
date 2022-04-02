﻿Imports VB_ChromaSampleApp.ChromaSDK
Imports System

Class SampleApp

    Private _mResult As Integer = 0
    Private _mRandom As Random = New Random()

    Private _mShortCode As String = ChromaSDK.Stream._Default.Shortcode
    Private _mLenShortCode As Byte = 0

    Private _mStreamId As String = ChromaSDK.Stream._Default.StreamId
    Private _mLenStreamId As Byte = 0

    Private _mStreamKey As String = ChromaSDK.Stream._Default.StreamKey
    Private _mLenStreamKey As Byte = 0

    Private _mStreamFocus As String = ChromaSDK.Stream._Default.StreamFocus
    Private _mLenStreamFocus As Byte = 0
    Private _mStreamFocusGuid As String = "UnitTest-" + Guid.NewGuid().ToString()

    Public Function GetInitResult() As Integer
        Return _mResult
    End Function

    Public Function GetShortcode() As String
        If _mLenShortCode.Equals(0) Then
            Return "NOT_SET"
        Else
            Return _mShortCode
        End If
    End Function


    Public Function GetStreamId() As String
        If _mLenStreamId.Equals(0) Then
            Return "NOT_SET"
        Else
            Return _mStreamId
        End If
    End Function

    Public Function GetStreamKey() As String
        If _mLenStreamKey.Equals(0) Then
            Return "NOT_SET"
        Else
            Return _mStreamKey
        End If
    End Function


    Public Function GetStreamFocus() As String
        If _mLenStreamFocus.Equals(0) Then
            Return "NOT_SET"
        Else
            Return _mStreamFocus
        End If
    End Function

    Public Function Start()
        Dim appInfo As ChromaSDK.APPINFOTYPE = New APPINFOTYPE()
        appInfo.Title = "Razer Chroma CSharp Sample Application"
        appInfo.Description = "A sample application using Razer Chroma SDK"

        appInfo.Author_Name = "Razer"
        appInfo.Author_Contact = "https://developer.razer.com/chroma"

        REM //appInfo.SupportedDevice = 
        REM //    0x01 | // Keyboards
        REM //    0x02 | // Mice
        REM //    0x04 | // Headset
        REM //    0x08 | // Mousepads
        REM //    0x10 | // Keypads
        REM //    0x20   // ChromaLink devices
        appInfo.SupportedDevice = (&H1 Xor &H2 Xor &H4 Xor &H8 Xor &H10 Xor &H20)
        REM //    0x01 | // Utility. (To specifiy this Is an utility application)
        REM //    0x02   // Game. (To specifiy this Is a game)
        appInfo.Category = 1
        _mResult = ChromaAnimationAPI.InitSDK(appInfo)
        Select Case (_mResult)
            Case RazerErrors.RZRESULT_DLL_NOT_FOUND
                Console.Error.WriteLine("Chroma DLL is not found! {0}", RazerErrors.GetResultString(_mResult))
            Case RazerErrors.RZRESULT_DLL_INVALID_SIGNATURE
                Console.Error.WriteLine("Chroma DLL has an invalid signature! {0}", RazerErrors.GetResultString(_mResult))
            Case RazerErrors.RZRESULT_SUCCESS
            Case Else
                Console.Error.WriteLine("Failed to initialize Chroma! {0}", RazerErrors.GetResultString(_mResult))
        End Select
        Return Nothing
    End Function

    Public Function OnApplicationQuit()
        ChromaAnimationAPI.Uninit()
        Return Nothing
    End Function

    Public Function GetEffectName(Index As Integer) As String
        Select Case (Index)
            Case -9
                Return "Request Shortcode\t"
            Case -8
                Return "Request StreamId\t"
            Case -7
                Return "Request StreamKey\t"
            Case -6
                Return "Release Shortcode\r\n"
            Case -5
                Return "Broadcast\t\t"
            Case -4
                Return "BroadcastEnd\r\n"
            Case -3
                Return "Watch\t\t"
            Case -2
                Return "WatchEnd\r\n"
            Case -1
                Return "GetFocus\t\t"
            Case 0
                Return "SetFocus\r\n"
            Case Else
                Return String.Format("Effect{0}", Index)
        End Select
    End Function

    Public Function ExecuteItem(index As Integer)
        Select Case (index)
            Case -9
                If ChromaAnimationAPI.CoreStreamSupportsStreaming() Then
                    _mShortCode = ChromaSDK.Stream._Default.Shortcode
                    _mLenShortCode = 0
                    ChromaAnimationAPI.CoreStreamGetAuthShortcode(_mShortCode, _mLenShortCode, "PC", "VB Sample App 好")
                End If
            Case -8
                If ChromaAnimationAPI.CoreStreamSupportsStreaming() Then
                    _mStreamId = ChromaSDK.Stream._Default.StreamId
                    _mLenStreamId = 0
                    ChromaAnimationAPI.CoreStreamGetId(_mShortCode, _mStreamId, _mLenStreamId)
                    If _mLenStreamId > 0 Then
                        _mStreamId = _mStreamId.Substring(0, _mLenStreamId)
                    End If
                End If
            Case -7
                If ChromaAnimationAPI.CoreStreamSupportsStreaming() Then
                    _mStreamKey = ChromaSDK.Stream._Default.StreamKey
                    _mLenStreamKey = 0
                    ChromaAnimationAPI.CoreStreamGetKey(_mShortCode, _mStreamKey, _mLenStreamKey)
                    If (_mLenStreamId > 0) Then
                        _mStreamKey = _mStreamKey.Substring(0, _mLenStreamKey)
                    End If
                End If
            Case -6
                If (ChromaAnimationAPI.CoreStreamSupportsStreaming() And ChromaAnimationAPI.CoreStreamReleaseShortcode(_mShortCode)) Then
                    _mShortCode = ChromaSDK.Stream._Default.Shortcode
                    _mLenShortCode = 0
                End If
            Case -5
                If (ChromaAnimationAPI.CoreStreamSupportsStreaming() And _mLenStreamId > 0 And _mLenStreamKey > 0) Then
                    ChromaAnimationAPI.CoreStreamBroadcast(_mStreamId, _mStreamKey)
                End If
            Case -4
                If ChromaAnimationAPI.CoreStreamSupportsStreaming() Then
                    ChromaAnimationAPI.CoreStreamBroadcastEnd()
                End If
            Case -3
                If (ChromaAnimationAPI.CoreStreamSupportsStreaming() And _mLenStreamId > 0) Then
                    ChromaAnimationAPI.CoreStreamWatch(_mStreamId, 0)
                End If
            Case -2
                If ChromaAnimationAPI.CoreStreamSupportsStreaming() Then
                    ChromaAnimationAPI.CoreStreamWatchEnd()
                End If
            Case -1
                If ChromaAnimationAPI.CoreStreamSupportsStreaming() Then
                    _mStreamFocus = ChromaSDK.Stream._Default.StreamFocus
                    _mLenStreamFocus = 0
                    ChromaAnimationAPI.CoreStreamGetFocus(_mStreamFocus, _mLenStreamFocus)
                End If
            Case 0
                If ChromaAnimationAPI.CoreStreamSupportsStreaming() Then
                    ChromaAnimationAPI.CoreStreamSetFocus(_mStreamFocusGuid)
                    _mStreamFocus = ChromaSDK.Stream._Default.StreamFocus
                    _mLenStreamFocus = 0
                    ChromaAnimationAPI.CoreStreamGetFocus(_mStreamFocus, _mLenStreamFocus)
                End If
        End Select
        Return Nothing
    End Function

End Class