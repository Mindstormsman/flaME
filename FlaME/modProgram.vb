﻿Imports ICSharpCode.SharpZipLib

Public Module modProgram

    Public Const ProgramName As String = "FlaME"

    Public Const ProgramVersionNumber As String = "1.20"

#If MonoDevelop = 0.0# Then
    Public Const ProgramPlatform As String = "Windows"
#Else
    #If Mono <> 0.0# Then
        Public Const ProgramPlatform As String = "Mono 2.10.1"
    #Else
        Public Const ProgramPlatform As String = "MonoDevelop Microsoft .NET"
    #End If 
#End If

    Public Const PlayerCountMax As Integer = 10

    Public Const DefaultHeightMultiplier As Integer = 2

#If Mono = 0.0# Then
    Public Const MinimapDelay As Integer = 100
#Else
    Public Const MinimapDelay As Integer = 1000
#End If

    Public Const SectorTileSize As Integer = 8

    Public Const FOVDefault As Double = 30.0# / (50.0# * 900.0#) ' screen_vertical_size / ( screen_dist * screen_vertical_pixels )

    Public Const MaxDroidWeapons As Integer = 3

    Public Const WZMapMaxSize As Integer = 250
    Public Const MapMaxSize As Integer = 512

    Public Const MinimapMaxSize As Integer = 512

    Public OSPathSeperator As Char

    Public MyDocumentsProgramPath As String

    Public SettingsPath As String
    Public OldSettingsPath As String
    Public AutoSavePath As String
    Public TilesetsPath As String
    Public ObjectDataPath As String
#If MonoDevelop <> 0.0# Then
    Public InterfaceImagesPath As String
#End If

    Public Sub SetProgramSubDirs()

        MyDocumentsProgramPath = My.Computer.FileSystem.SpecialDirectories.MyDocuments & OSPathSeperator & ".flaME"
        SettingsPath = MyDocumentsProgramPath & OSPathSeperator & "settings.ini"
        OldSettingsPath = MyDocumentsProgramPath & OSPathSeperator & "settings"
        AutoSavePath = MyDocumentsProgramPath & OSPathSeperator & "autosave" & OSPathSeperator
        TilesetsPath = MyDocumentsProgramPath & OSPathSeperator & "tilesets" & OSPathSeperator
        ObjectDataPath = MyDocumentsProgramPath & OSPathSeperator & "objectdata" & OSPathSeperator
#If MonoDevelop <> 0.0# Then
        InterfaceImagesPath = My.Application.Info.DirectoryPath & OSPathSeperator & "interface" & OSPathSeperator
#End If
    End Sub

    Public ProgramIcon As Icon

    Public MinimapSize As Integer = 160

    Public Undo_Limit As UInteger = 256UI

    Public AutoSave_MinInterval_s As UInteger = 180UI
    Public AutoSave_MinChanges As UInteger = 20UI

    Public DirectPointer As Boolean = True

    Public GLTexture_NoTile As Integer
    Public GLTexture_OverflowTile As Integer

    Public frmMainInstance As New frmMain
#If MonoDevelop = 0.0# Then
    Public frmSplashInstance As New frmSplash
#End If
    Public frmCompileInstance As New frmCompile
    Public frmMapTexturerInstance As New frmMapTexturer
    Public frmGeneratorInstance As New frmGenerator

    Public IsViewKeyDown(255) As Boolean

    'interface controls
    Public Control_Deselect As clsInputControl
    'selected unit controls
    Public Control_Unit_Move As clsInputControl
    Public Control_Unit_Delete As clsInputControl
    Public Control_Unit_Multiselect As clsInputControl
    'generalised controls
    Public Control_Slow As clsInputControl
    Public Control_Fast As clsInputControl
    'picker controls
    Public Control_Picker As clsInputControl
    'view controls
    Public Control_View_Textures As clsInputControl
    Public Control_View_Lighting As clsInputControl
    Public Control_View_Wireframe As clsInputControl
    Public Control_View_Units As clsInputControl
    Public Control_View_Move_Type As clsInputControl
    Public Control_View_Rotate_Type As clsInputControl
    Public Control_View_Move_Left As clsInputControl
    Public Control_View_Move_Right As clsInputControl
    Public Control_View_Move_Forward As clsInputControl
    Public Control_View_Move_Backward As clsInputControl
    Public Control_View_Move_Up As clsInputControl
    Public Control_View_Move_Down As clsInputControl
    Public Control_View_Zoom_In As clsInputControl
    Public Control_View_Zoom_Out As clsInputControl
    Public Control_View_Left As clsInputControl
    Public Control_View_Right As clsInputControl
    Public Control_View_Forward As clsInputControl
    Public Control_View_Backward As clsInputControl
    Public Control_View_Up As clsInputControl
    Public Control_View_Down As clsInputControl
    Public Control_View_Reset As clsInputControl
    Public Control_View_Roll_Left As clsInputControl
    Public Control_View_Roll_Right As clsInputControl
    'texture controls
    Public Control_Clockwise As clsInputControl
    Public Control_Anticlockwise As clsInputControl
    Public Control_Texture_Flip As clsInputControl
    Public Control_Tri_Flip As clsInputControl
    'gateway controls
    Public Control_Gateway_Delete As clsInputControl
    'undo controls
    Public Control_Undo As clsInputControl
    Public Control_Redo As clsInputControl

    Public TextureBrush As New clsBrush(0.0#, clsBrush.enumShape.Circle)
    Public TerrainBrush As New clsBrush(2.0#, clsBrush.enumShape.Circle)
    Public HeightBrush As New clsBrush(2.0#, clsBrush.enumShape.Circle)
    Public CliffBrush As New clsBrush(2.0#, clsBrush.enumShape.Circle)

    Public SmoothRadius As New clsBrush(1.5#, clsBrush.enumShape.Circle)

    Public InputControls() As clsInputControl
    Public InputControlCount As Integer

    Public DisplayTileOrientation As Boolean

    Public Enum enumTool As Byte
        None
        Texture_Brush
        AutoTexture_Place
        AutoTexture_Fill
        AutoRoad_Place
        AutoRoad_Line
        AutoRoad_Remove
        AutoCliff
        Height_Set_Brush
        Height_Change_Brush
        Height_Smooth_Brush
        ObjectPlace
        Terrain_Select
        Gateways
    End Enum
    Public Tool As enumTool = enumTool.Texture_Brush

    Public SelectedTextureNum As Integer = -1
    Public TextureOrientation As New sTileOrientation(False, False, False)

    Public SelectedTerrain As clsPainter.clsTerrain
    Public SelectedRoad As clsPainter.clsRoad

    Public Structure sTileType
        Dim Name As String
        Dim DisplayColour As sRGB_sng
    End Structure
    Public TileTypes(-1) As sTileType
    Public TileTypeCount As Integer

    Public Const TileTypeNum_Water As Integer = 7
    Public Const TileTypeNum_Cliff As Integer = 8

    Public TemplateDroidTypes(-1) As clsDroidDesign.clsTemplateDroidType
    Public TemplateDroidTypeCount As Integer

    Public Enum enumObjectRotateMode As Byte
        None
        Walls
        All
    End Enum

    Public Structure sResult
        Dim Success As Boolean
        Dim Problem As String
    End Structure

    Public Structure sTextLabel
        Dim Text As String
        Dim Font As GLFont
        Dim SizeY As Single
        Dim Colour As sRGBA_sng
        Dim Pos As sXY_int

        Public Function GetSizeX() As Single
            Dim SizeX As Single
            Dim CharWidth As Single
            Dim CharSpacing As Single = SizeY / 10.0F
            Dim CharSize As Single = SizeY / Font.Height
            Dim A As Integer

            For A = 0 To Text.Length - 1
                CharWidth = Font.Character(Asc(Text.Chars(A))).Width * CharSize
                SizeX += CharWidth
            Next
            SizeX += CharSpacing * (Text.Length - 1)

            Return SizeX
        End Function
    End Structure

    Public Structure sWZAngle
        Public Direction As UShort
        Public Pitch As UShort
        Public Roll As UShort
    End Structure

    Public Structure sRGB_sng
        Dim Red As Single
        Dim Green As Single
        Dim Blue As Single

        Public Sub New(ByVal Red As Single, ByVal Green As Single, ByVal Blue As Single)

            Me.Red = Red
            Me.Green = Green
            Me.Blue = Blue
        End Sub
    End Structure

    Public Structure sRGBA_sng
        Dim Red As Single
        Dim Green As Single
        Dim Blue As Single
        Dim Alpha As Single

        Public Sub New(ByVal Red As Single, ByVal Green As Single, ByVal Blue As Single, ByVal Alpha As Single)

            Me.Red = Red
            Me.Green = Green
            Me.Blue = Blue
            Me.Alpha = Alpha
        End Sub
    End Structure

    Public Class clsBrush

        Private _Radius As Double
        Public Enum enumShape As Byte
            Circle
            Square
        End Enum
        Private _Shape As enumShape = enumShape.Circle

        Public Tiles As sBrushTiles

        Public Property Radius As Double
            Get
                Return _Radius
            End Get
            Set(ByVal value As Double)
                If _Radius = value Then
                    Exit Property
                End If
                _Radius = value
                CreateTiles()
            End Set
        End Property

        Public Property Shape As enumShape
            Get
                Return _Shape
            End Get
            Set(ByVal value As enumShape)
                If _Shape = value Then
                    Exit Property
                End If
                _Shape = value
                CreateTiles()
            End Set
        End Property

        Private Sub CreateTiles()

            Select Case _Shape
                Case enumShape.Circle
                    CircleTiles_Create(_Radius, Tiles, 1.0#)
                Case enumShape.Square
                    SquareTiles_Create(_Radius, Tiles, 1.0#)
            End Select
        End Sub

        Public Sub New(ByVal InitialRadius As Double, ByVal InitialShape As enumShape)

            _Radius = InitialRadius
            _Shape = InitialShape
            CreateTiles()
        End Sub
    End Class

    Public Structure sBrushTiles
        Dim XMin() As Integer
        Dim XMax() As Integer
        Dim YMin As Integer
        Dim YMax As Integer
    End Structure

    Public TerrainGridSpacing As Integer = 128

    Public VisionRadius_2E As Integer
    Public VisionRadius As Double

    Public Main_Map As clsMap

    Public Copied_Map As clsMap

    Public Tilesets() As clsTileset
    Public TilesetCount As Integer

    Public Tileset_Arizona As clsTileset
    Public Tileset_Urban As clsTileset
    Public Tileset_Rockies As clsTileset

    Public Painter_Arizona As clsPainter
    Public Painter_Urban As clsPainter
    Public Painter_Rockies As clsPainter

    Public UnitTypes(-1) As clsUnitType
    Public UnitTypeCount As Integer

    Public Structure sTexturePage
        Dim FileTitle As String
        Dim GLTexture_Num As Integer
    End Structure
    Public TexturePages() As sTexturePage
    Public TexturePageCount As Integer

    Public UnitLabelFont As GLFont
    Public TextureViewFont As GLFont
    Public UnitLabelFontSize As Single

    Public PlayerColour(15) As sRGB_sng

    Public Structure sSplitPath

        Public Parts() As String
        Public PartCount As Integer
        Public FilePath As String
        Public FileTitle As String
        Public FileTitleWithoutExtension As String
        Public FileExtension As String

        Sub New(ByVal Path As String)
            Dim A As Integer

            Parts = Path.Split(OSPathSeperator)
            PartCount = Parts.GetUpperBound(0) + 1
            FilePath = ""
            For A = 0 To PartCount - 2
                FilePath &= Parts(A) & OSPathSeperator
            Next
            FileTitle = Parts(A)
            A = InStrRev(FileTitle, ".")
            If A > 0 Then
                FileExtension = Strings.Right(FileTitle, FileTitle.Length - A)
                FileTitleWithoutExtension = Strings.Left(FileTitle, A - 1)
            Else
                FileExtension = ""
                FileTitleWithoutExtension = FileTitle
            End If
        End Sub
    End Structure

    Public Structure sZipSplitPath

        Public Parts() As String
        Public PartCount As Integer
        Public FilePath As String
        Public FileTitle As String
        Public FileTitleWithoutExtension As String
        Public FileExtension As String

        Sub New(ByVal Path As String)
            Dim tmpPath As String = Path.ToLower.Replace("\"c, "/"c)
            Dim A As Integer

            Parts = tmpPath.Split("/"c)
            PartCount = Parts.GetUpperBound(0) + 1
            FilePath = ""
            For A = 0 To PartCount - 2
                FilePath &= Parts(A) & "/"
            Next
            FileTitle = Parts(A)
            A = InStrRev(FileTitle, ".")
            If A > 0 Then
                FileExtension = Strings.Right(FileTitle, FileTitle.Length - A)
                FileTitleWithoutExtension = Strings.Left(FileTitle, A - 1)
            Else
                FileExtension = ""
                FileTitleWithoutExtension = FileTitle
            End If
        End Sub
    End Structure

    Public Sub VisionRadius_2E_Changed()

        VisionRadius = 256.0# * 2.0# ^ (VisionRadius_2E / 2.0#)
        If frmMainInstance.View IsNot Nothing Then
            frmMainInstance.View.View_Radius_Set(VisionRadius)
            frmMainInstance.View.DrawViewLater()
        End If
    End Sub

    Public Sub CircleTiles_Create(ByVal Radius As Double, ByRef Output As sBrushTiles, ByVal TileSize As Double)
        Dim X As Double
        Dim Y As Integer
        Dim Radius2 As Double
        Dim Radius3 As Double
        Dim A As Integer

        Radius2 = Radius / TileSize
        Output.YMax = Math.Floor(Radius2)
        Output.YMin = -Output.YMax
        ReDim Output.XMin(Output.YMax - Output.YMin)
        ReDim Output.XMax(Output.YMax - Output.YMin)
        Radius3 = Radius2 * Radius2
        For Y = Output.YMin To Output.YMax
            X = Math.Sqrt(Radius3 - Y * Y)
            A = Y - Output.YMin
            Output.XMax(A) = Math.Floor(X)
            Output.XMin(A) = -Output.XMax(A)
        Next
    End Sub

    Public Sub SquareTiles_Create(ByVal Radius As Double, ByRef Output As sBrushTiles, ByVal TileSize As Double)
        Dim Y As Integer
        Dim A As Integer
        Dim B As Integer

        A = Math.Floor(Radius / TileSize)
        Output.YMin = -A
        Output.YMax = A
        B = A * 2
        ReDim Output.XMin(B)
        ReDim Output.XMax(B)
        For Y = 0 To B
            Output.XMin(Y) = -A
            Output.XMax(Y) = A
        Next
    End Sub

    Public Function Get_TexturePage_GLTexture(ByVal FileTitle As String) As Integer
        Dim LCaseTitle As String = LCase(FileTitle)
        Dim A As Integer

        For A = 0 To TexturePageCount - 1
            If LCase(TexturePages(A).FileTitle) = LCaseTitle Then
                Return TexturePages(A).GLTexture_Num
            End If
        Next
        Return 0
    End Function

    Public Sub XY_Reorder(ByVal XY_A_Input As sXY_int, ByVal XY_B_Input As sXY_int, ByRef XY_Low_Output As sXY_int, ByRef XY_High_Output As sXY_int)

        If XY_A_Input.X <= XY_B_Input.X Then
            XY_Low_Output.X = XY_A_Input.X
            XY_High_Output.X = XY_B_Input.X
        Else
            XY_Low_Output.X = XY_B_Input.X
            XY_High_Output.X = XY_A_Input.X
        End If
        If XY_A_Input.Y <= XY_B_Input.Y Then
            XY_Low_Output.Y = XY_A_Input.Y
            XY_High_Output.Y = XY_B_Input.Y
        Else
            XY_Low_Output.Y = XY_B_Input.Y
            XY_High_Output.Y = XY_A_Input.Y
        End If
    End Sub

    Public Function EndWithPathSeperator(ByVal Text As String) As String

        If Strings.Right(Text, 1) = OSPathSeperator Then
            EndWithPathSeperator = Text
        Else
            EndWithPathSeperator = Text & OSPathSeperator
        End If
    End Function

    Public Function TileType_Add(ByVal NewTileType As sTileType) As Integer

        ReDim Preserve TileTypes(TileTypeCount)
        TileTypes(TileTypeCount) = NewTileType
        TileType_Add = TileTypeCount
        TileTypeCount += 1
    End Function

    Public Function MinDigits(ByVal Number As Integer, ByVal Digits As Integer) As String
        Dim A As Integer

        MinDigits = Number
        A = Digits - MinDigits.Length
        If A > 0 Then
            MinDigits = Strings.StrDup(A, "0"c) & MinDigits
        End If
    End Function

    Public Sub ViewKeyDown_Clear()
        Dim A As Integer

        For A = 0 To 255
            IsViewKeyDown(A) = False
        Next A

        For A = 0 To InputControlCount - 1
            InputControls(A).KeysChanged(IsViewKeyDown)
        Next
    End Sub

    Public Function OSRGB(ByVal Red As Integer, ByVal Green As Integer, ByVal Blue As Integer) As Integer

#If Mono = 0.0# And Mono267 = 0.0# Then
        OSRGB = RGB(Red, Green, Blue)
#Else
        OSRGB = RGB(Blue, Green, Red)
#End If
    End Function

    Public Function LoseMapQuestion() As Boolean

        Return (MsgBox("Lose any unsaved changes to this map?", MsgBoxStyle.Question + MsgBoxStyle.OkCancel, "") = MsgBoxResult.Ok)
    End Function

    Public Sub CreatePainterArizona()
        Dim NewBrushCliff As clsPainter.sCliff_Brush
        Dim NewBrush As clsPainter.sTransition_Brush
        Dim NewRoadBrush As clsPainter.sRoad_Brush

        Painter_Arizona = New clsPainter

        'arizona

        Dim Terrain_Red As New clsPainter.clsTerrain
        Terrain_Red.Name = "Red"
        Painter_Arizona.Terrain_Add(Terrain_Red)

        Dim Terrain_Yellow As New clsPainter.clsTerrain
        Terrain_Yellow.Name = "Yellow"
        Painter_Arizona.Terrain_Add(Terrain_Yellow)

        Dim Terrain_Sand As New clsPainter.clsTerrain
        Terrain_Sand.Name = "Sand"
        Painter_Arizona.Terrain_Add(Terrain_Sand)

        Dim Terrain_Brown As New clsPainter.clsTerrain
        Terrain_Brown.Name = "Brown"
        Painter_Arizona.Terrain_Add(Terrain_Brown)

        Dim Terrain_Green As New clsPainter.clsTerrain
        Terrain_Green.Name = "Green"
        Painter_Arizona.Terrain_Add(Terrain_Green)

        Dim Terrain_Concrete As New clsPainter.clsTerrain
        Terrain_Concrete.Name = "Concrete"
        Painter_Arizona.Terrain_Add(Terrain_Concrete)

        Dim Terrain_Water As New clsPainter.clsTerrain
        Terrain_Water.Name = "Water"
        Painter_Arizona.Terrain_Add(Terrain_Water)

        'red centre brush
        Terrain_Red.Tiles.Tile_Add(48, TileDirection_None, 1)
        Terrain_Red.Tiles.Tile_Add(53, TileDirection_None, 1)
        Terrain_Red.Tiles.Tile_Add(54, TileDirection_None, 1)
        Terrain_Red.Tiles.Tile_Add(76, TileDirection_None, 1)
        'yellow centre brushTerrain_yellow
        Terrain_Yellow.Tiles.Tile_Add(9, TileDirection_None, 1)
        Terrain_Yellow.Tiles.Tile_Add(11, TileDirection_None, 1)
        'sand centre brush
        Terrain_Sand.Tiles.Tile_Add(12, TileDirection_None, 1)
        'brown centre brush
        Terrain_Brown.Tiles.Tile_Add(5, TileDirection_None, 1)
        Terrain_Brown.Tiles.Tile_Add(6, TileDirection_None, 1)
        Terrain_Brown.Tiles.Tile_Add(7, TileDirection_None, 1)
        Terrain_Brown.Tiles.Tile_Add(8, TileDirection_None, 1)
        'green centre brush
        Terrain_Green.Tiles.Tile_Add(23, TileDirection_None, 1)
        'concrete centre brush
        Terrain_Concrete.Tiles.Tile_Add(22, TileDirection_None, 1)
        'water centre brush
        Terrain_Water.Tiles.Tile_Add(17, TileDirection_None, 1)
        'red cliff brush
        NewBrushCliff = New clsPainter.sCliff_Brush
        NewBrushCliff.Name = "Red Cliff"
        NewBrushCliff.Terrain_Inner = Terrain_Red
        NewBrushCliff.Terrain_Outer = Terrain_Red
        NewBrushCliff.Tiles_Straight.Tile_Add(46, TileDirection_Bottom, 1)
        NewBrushCliff.Tiles_Straight.Tile_Add(71, TileDirection_Bottom, 1)
        NewBrushCliff.Tiles_Corner_In.Tile_Add(45, TileDirection_TopRight, 1)
        NewBrushCliff.Tiles_Corner_In.Tile_Add(75, TileDirection_TopLeft, 1)
        NewBrushCliff.Tiles_Corner_Out.Tile_Add(45, TileDirection_BottomLeft, 1)
        NewBrushCliff.Tiles_Corner_Out.Tile_Add(75, TileDirection_BottomRight, 1)
        Painter_Arizona.CliffBrush_Add(NewBrushCliff)
        'water to sand transition brush
        NewBrush = New clsPainter.sTransition_Brush
        NewBrush.Name = "Water->Sand"
        NewBrush.Terrain_Inner = Terrain_Water
        NewBrush.Terrain_Outer = Terrain_Sand
        NewBrush.Tiles_Straight.Tile_Add(14, TileDirection_Bottom, 1)
        NewBrush.Tiles_Corner_In.Tile_Add(16, TileDirection_BottomLeft, 1)
        NewBrush.Tiles_Corner_Out.Tile_Add(15, TileDirection_BottomLeft, 1)
        Painter_Arizona.TransitionBrush_Add(NewBrush)
        'water to green transition brush
        NewBrush = New clsPainter.sTransition_Brush
        NewBrush.Name = "Water->Green"
        NewBrush.Terrain_Inner = Terrain_Water
        NewBrush.Terrain_Outer = Terrain_Green
        NewBrush.Tiles_Straight.Tile_Add(31, TileDirection_Top, 1)
        NewBrush.Tiles_Corner_In.Tile_Add(33, TileDirection_TopLeft, 1)
        NewBrush.Tiles_Corner_Out.Tile_Add(32, TileDirection_TopLeft, 1)
        Painter_Arizona.TransitionBrush_Add(NewBrush)
        'yellow to red transition brush
        NewBrush = New clsPainter.sTransition_Brush
        NewBrush.Name = "Yellow->Red"
        NewBrush.Terrain_Inner = Terrain_Yellow
        NewBrush.Terrain_Outer = Terrain_Red
        NewBrush.Tiles_Straight.Tile_Add(27, TileDirection_Right, 1)
        NewBrush.Tiles_Corner_In.Tile_Add(28, TileDirection_BottomRight, 1)
        NewBrush.Tiles_Corner_Out.Tile_Add(29, TileDirection_BottomRight, 1)
        Painter_Arizona.TransitionBrush_Add(NewBrush)
        'sand to red transition brush
        NewBrush = New clsPainter.sTransition_Brush
        NewBrush.Name = "Sand->Red"
        NewBrush.Terrain_Inner = Terrain_Sand
        NewBrush.Terrain_Outer = Terrain_Red
        NewBrush.Tiles_Straight.Tile_Add(43, TileDirection_Left, 1)
        NewBrush.Tiles_Corner_In.Tile_Add(42, TileDirection_TopLeft, 1)
        NewBrush.Tiles_Corner_Out.Tile_Add(41, TileDirection_TopLeft, 1)
        Painter_Arizona.TransitionBrush_Add(NewBrush)
        'sand to yellow transition brush
        NewBrush = New clsPainter.sTransition_Brush
        NewBrush.Name = "Sand->Yellow"
        NewBrush.Terrain_Inner = Terrain_Sand
        NewBrush.Terrain_Outer = Terrain_Yellow
        NewBrush.Tiles_Straight.Tile_Add(10, TileDirection_Left, 1)
        NewBrush.Tiles_Corner_In.Tile_Add(1, TileDirection_TopLeft, 1)
        NewBrush.Tiles_Corner_Out.Tile_Add(0, TileDirection_TopLeft, 1)
        Painter_Arizona.TransitionBrush_Add(NewBrush)
        'brown to red transition brush
        NewBrush = New clsPainter.sTransition_Brush
        NewBrush.Name = "Brown->Red"
        NewBrush.Terrain_Inner = Terrain_Brown
        NewBrush.Terrain_Outer = Terrain_Red
        NewBrush.Tiles_Straight.Tile_Add(34, TileDirection_Left, 1)
        NewBrush.Tiles_Corner_In.Tile_Add(36, TileDirection_TopLeft, 1)
        NewBrush.Tiles_Corner_Out.Tile_Add(35, TileDirection_TopLeft, 1)
        Painter_Arizona.TransitionBrush_Add(NewBrush)
        'brown to yellow transition brush
        NewBrush = New clsPainter.sTransition_Brush
        NewBrush.Name = "Brown->Yellow"
        NewBrush.Terrain_Inner = Terrain_Brown
        NewBrush.Terrain_Outer = Terrain_Yellow
        NewBrush.Tiles_Straight.Tile_Add(38, TileDirection_Left, 1)
        NewBrush.Tiles_Corner_In.Tile_Add(39, TileDirection_BottomRight, 1)
        NewBrush.Tiles_Corner_Out.Tile_Add(40, TileDirection_BottomRight, 1)
        Painter_Arizona.TransitionBrush_Add(NewBrush)
        'brown to sand transition brush
        NewBrush = New clsPainter.sTransition_Brush
        NewBrush.Name = "Brown->Sand"
        NewBrush.Terrain_Inner = Terrain_Brown
        NewBrush.Terrain_Outer = Terrain_Sand
        NewBrush.Tiles_Straight.Tile_Add(2, TileDirection_Left, 1)
        NewBrush.Tiles_Corner_In.Tile_Add(3, TileDirection_BottomRight, 1)
        NewBrush.Tiles_Corner_Out.Tile_Add(4, TileDirection_BottomRight, 1)
        Painter_Arizona.TransitionBrush_Add(NewBrush)
        'brown to green transition brush
        NewBrush = New clsPainter.sTransition_Brush
        NewBrush.Name = "Brown->Green"
        NewBrush.Terrain_Inner = Terrain_Brown
        NewBrush.Terrain_Outer = Terrain_Green
        NewBrush.Tiles_Straight.Tile_Add(24, TileDirection_Left, 1)
        NewBrush.Tiles_Corner_In.Tile_Add(26, TileDirection_TopLeft, 1)
        NewBrush.Tiles_Corner_Out.Tile_Add(25, TileDirection_TopLeft, 1)
        Painter_Arizona.TransitionBrush_Add(NewBrush)
        'concrete to red transition brush
        NewBrush = New clsPainter.sTransition_Brush
        NewBrush.Name = "Concrete->Red"
        NewBrush.Terrain_Inner = Terrain_Concrete
        NewBrush.Terrain_Outer = Terrain_Red
        NewBrush.Tiles_Straight.Tile_Add(21, TileDirection_Right, 1)
        NewBrush.Tiles_Corner_In.Tile_Add(19, TileDirection_BottomRight, 1)
        NewBrush.Tiles_Corner_Out.Tile_Add(20, TileDirection_BottomRight, 1)
        Painter_Arizona.TransitionBrush_Add(NewBrush)


        Dim Road_Road As New clsPainter.clsRoad

        Road_Road = New clsPainter.clsRoad
        Road_Road.Name = "Road"
        Painter_Arizona.Road_Add(Road_Road)

        Dim Road_Track As New clsPainter.clsRoad

        Road_Track = New clsPainter.clsRoad
        Road_Track.Name = "Track"
        Painter_Arizona.Road_Add(Road_Track)

        'road
        NewRoadBrush = New clsPainter.sRoad_Brush
        NewRoadBrush.Road = Road_Road
        NewRoadBrush.Terrain = Terrain_Red
        NewRoadBrush.Tile_TIntersection.Tile_Add(57, TileDirection_Bottom, 1)
        NewRoadBrush.Tile_Straight.Tile_Add(59, TileDirection_Left, 1)
        NewRoadBrush.Tile_End.Tile_Add(47, TileDirection_Left, 1)
        Painter_Arizona.RoadBrush_Add(NewRoadBrush)
        'track
        NewRoadBrush = New clsPainter.sRoad_Brush
        NewRoadBrush.Road = Road_Track
        NewRoadBrush.Terrain = Terrain_Red
        NewRoadBrush.Tile_CrossIntersection.Tile_Add(73, TileDirection_None, 1)
        NewRoadBrush.Tile_TIntersection.Tile_Add(72, TileDirection_Right, 1)
        NewRoadBrush.Tile_Straight.Tile_Add(49, TileDirection_Top, 1)
        NewRoadBrush.Tile_Straight.Tile_Add(51, TileDirection_Top, 2)
        NewRoadBrush.Tile_Corner_In.Tile_Add(50, TileDirection_BottomRight, 1)
        NewRoadBrush.Tile_End.Tile_Add(52, TileDirection_Bottom, 1)
        Painter_Arizona.RoadBrush_Add(NewRoadBrush)

        With Generator_TilesetArizona
            ReDim .OldTextureLayers.Layers(-1)
            .OldTextureLayers.LayerCount = 0
        End With

        Dim NewLayer As frmMapTexturer.sLayerList.clsLayer

        NewLayer = New frmMapTexturer.sLayerList.clsLayer
        With Generator_TilesetArizona
            .OldTextureLayers.Layer_Insert(.OldTextureLayers.LayerCount, NewLayer)
        End With
        With NewLayer
            .Terrain = Terrain_Red
            .HeightMax = 255.0F
            .SlopeMax = RadOf90Deg
            .Scale = 0.0F
            .Density = 1.0F
        End With

        NewLayer = New frmMapTexturer.sLayerList.clsLayer
        With Generator_TilesetArizona
            .OldTextureLayers.Layer_Insert(.OldTextureLayers.LayerCount, NewLayer)
        End With
        With NewLayer
            .Terrain = Terrain_Sand
            .HeightMax = -1.0F 'signals water distribution
            .SlopeMax = -1.0F 'signals water distribution
            .Scale = 0.0F
            .Density = 1.0F
        End With

        NewLayer = New frmMapTexturer.sLayerList.clsLayer
        With Generator_TilesetArizona
            .OldTextureLayers.Layer_Insert(.OldTextureLayers.LayerCount, NewLayer)
        End With
        With NewLayer
            .WithinLayer = 1
            .Terrain = Terrain_Water
            .HeightMax = 255.0F
            .SlopeMax = RadOf90Deg
            .Scale = 0.0F
            .Density = 1.0F
        End With

        NewLayer = New frmMapTexturer.sLayerList.clsLayer
        With Generator_TilesetArizona
            .OldTextureLayers.Layer_Insert(.OldTextureLayers.LayerCount, NewLayer)
        End With
        With NewLayer
            .AvoidLayers(1) = True
            .Terrain = Terrain_Brown
            .HeightMax = 255.0F
            .SlopeMax = -1.0F 'signals to use cliff angle
            .Scale = 3.0F
            .Density = 0.35F
        End With

        NewLayer = New frmMapTexturer.sLayerList.clsLayer
        With Generator_TilesetArizona
            .OldTextureLayers.Layer_Insert(.OldTextureLayers.LayerCount, NewLayer)
        End With
        With NewLayer
            .AvoidLayers(1) = True
            .AvoidLayers(3) = True
            .Terrain = Terrain_Yellow
            .HeightMax = 255.0F
            .SlopeMax = -1.0F 'signals to use cliff angle
            .Scale = 2.0F
            .Density = 0.6F
        End With

        NewLayer = New frmMapTexturer.sLayerList.clsLayer
        With Generator_TilesetArizona
            .OldTextureLayers.Layer_Insert(.OldTextureLayers.LayerCount, NewLayer)
        End With
        With NewLayer
            .AvoidLayers(1) = True
            .WithinLayer = 4
            .Terrain = Terrain_Sand
            .HeightMax = 255.0F
            .SlopeMax = RadOf90Deg
            .Scale = 1.0F
            .Density = 0.5F
        End With

        NewLayer = New frmMapTexturer.sLayerList.clsLayer
        With Generator_TilesetArizona
            .OldTextureLayers.Layer_Insert(.OldTextureLayers.LayerCount, NewLayer)
        End With
        With NewLayer
            .AvoidLayers(1) = True
            .WithinLayer = 3
            .Terrain = Terrain_Green
            .HeightMax = 255.0F
            .SlopeMax = RadOf90Deg
            .Scale = 2.0F
            .Density = 0.4F
        End With
    End Sub

    Public Sub CreatePainterUrban()
        Dim NewBrushCliff As clsPainter.sCliff_Brush
        Dim NewBrush As clsPainter.sTransition_Brush
        Dim NewRoadBrush As clsPainter.sRoad_Brush

        'urban

        Painter_Urban = New clsPainter

        Dim Terrain_Green As New clsPainter.clsTerrain
        Terrain_Green.Name = "Green"
        Painter_Urban.Terrain_Add(Terrain_Green)

        Dim Terrain_Blue As New clsPainter.clsTerrain
        Terrain_Blue.Name = "Blue"
        Painter_Urban.Terrain_Add(Terrain_Blue)

        Dim Terrain_Gray As New clsPainter.clsTerrain
        Terrain_Gray.Name = "Gray"
        Painter_Urban.Terrain_Add(Terrain_Gray)

        Dim Terrain_Orange As New clsPainter.clsTerrain
        Terrain_Orange.Name = "Orange"
        Painter_Urban.Terrain_Add(Terrain_Orange)

        Dim Terrain_Concrete As New clsPainter.clsTerrain
        Terrain_Concrete.Name = "Concrete"
        Painter_Urban.Terrain_Add(Terrain_Concrete)

        Dim Terrain_Water As New clsPainter.clsTerrain
        Terrain_Water.Name = "Water"
        Painter_Urban.Terrain_Add(Terrain_Water)

        'green centre brush
        Terrain_Green.Tiles.Tile_Add(50, TileDirection_None, 1)
        'blue centre brush
        Terrain_Blue.Tiles.Tile_Add(0, TileDirection_None, 14)
        Terrain_Blue.Tiles.Tile_Add(2, TileDirection_None, 1) 'line
        'gray centre brush
        Terrain_Gray.Tiles.Tile_Add(5, TileDirection_None, 1)
        Terrain_Gray.Tiles.Tile_Add(7, TileDirection_None, 4)
        Terrain_Gray.Tiles.Tile_Add(8, TileDirection_None, 4)
        Terrain_Gray.Tiles.Tile_Add(78, TileDirection_None, 4)
        'orange centre brush
        Terrain_Orange.Tiles.Tile_Add(31, TileDirection_None, 1) 'pipe
        Terrain_Orange.Tiles.Tile_Add(22, TileDirection_None, 50)
        'concrete centre brush
        Terrain_Concrete.Tiles.Tile_Add(51, TileDirection_None, 200)
        'water centre brush
        Terrain_Water.Tiles.Tile_Add(17, TileDirection_None, 1)
        'cliff brush
        NewBrushCliff = New clsPainter.sCliff_Brush
        NewBrushCliff.Name = "Cliff"
        NewBrushCliff.Terrain_Inner = Terrain_Gray
        NewBrushCliff.Terrain_Outer = Terrain_Gray
        NewBrushCliff.Tiles_Straight.Tile_Add(69, TileDirection_Bottom, 1)
        NewBrushCliff.Tiles_Straight.Tile_Add(70, TileDirection_Bottom, 1)
        NewBrushCliff.Tiles_Corner_In.Tile_Add(68, TileDirection_TopRight, 1)
        NewBrushCliff.Tiles_Corner_Out.Tile_Add(68, TileDirection_BottomLeft, 1)
        Painter_Urban.CliffBrush_Add(NewBrushCliff)
        'water to gray transition brush
        NewBrush = New clsPainter.sTransition_Brush
        NewBrush.Name = "Water->Gray"
        NewBrush.Terrain_Inner = Terrain_Water
        NewBrush.Terrain_Outer = Terrain_Gray
        NewBrush.Tiles_Straight.Tile_Add(23, TileDirection_Left, 1)
        NewBrush.Tiles_Straight.Tile_Add(24, TileDirection_Top, 1)
        NewBrush.Tiles_Corner_In.Tile_Add(25, TileDirection_TopLeft, 1)
        NewBrush.Tiles_Corner_Out.Tile_Add(26, TileDirection_TopLeft, 1)
        Painter_Urban.TransitionBrush_Add(NewBrush)
        'water to concrete transition brush
        NewBrush = New clsPainter.sTransition_Brush
        NewBrush.Name = "Water->Concrete"
        NewBrush.Terrain_Inner = Terrain_Water
        NewBrush.Terrain_Outer = Terrain_Concrete
        NewBrush.Tiles_Straight.Tile_Add(13, TileDirection_Left, 1)
        NewBrush.Tiles_Straight.Tile_Add(14, TileDirection_Bottom, 1)
        NewBrush.Tiles_Corner_In.Tile_Add(16, TileDirection_BottomLeft, 1)
        NewBrush.Tiles_Corner_Out.Tile_Add(15, TileDirection_BottomLeft, 1)
        Painter_Urban.TransitionBrush_Add(NewBrush)
        'gray to blue transition brush
        NewBrush = New clsPainter.sTransition_Brush
        NewBrush.Name = "Gray->Blue"
        NewBrush.Terrain_Inner = Terrain_Gray
        NewBrush.Terrain_Outer = Terrain_Blue
        NewBrush.Tiles_Straight.Tile_Add(6, TileDirection_Left, 1)
        NewBrush.Tiles_Corner_In.Tile_Add(4, TileDirection_BottomRight, 1)
        NewBrush.Tiles_Corner_Out.Tile_Add(3, TileDirection_BottomRight, 1)
        Painter_Urban.TransitionBrush_Add(NewBrush)
        'concrete to gray transition brush
        NewBrush = New clsPainter.sTransition_Brush
        NewBrush.Name = "Concrete->Gray"
        NewBrush.Terrain_Inner = Terrain_Concrete
        NewBrush.Terrain_Outer = Terrain_Gray
        NewBrush.Tiles_Straight.Tile_Add(9, TileDirection_Left, 1)
        NewBrush.Tiles_Straight.Tile_Add(27, TileDirection_Right, 1)
        NewBrush.Tiles_Corner_In.Tile_Add(30, TileDirection_BottomLeft, 1)
        NewBrush.Tiles_Corner_Out.Tile_Add(10, TileDirection_BottomLeft, 1)
        NewBrush.Tiles_Corner_Out.Tile_Add(29, TileDirection_BottomLeft, 1)
        Painter_Urban.TransitionBrush_Add(NewBrush)
        'orange to blue transition brush
        NewBrush = New clsPainter.sTransition_Brush
        NewBrush.Name = "Orange->Blue"
        NewBrush.Terrain_Inner = Terrain_Orange
        NewBrush.Terrain_Outer = Terrain_Blue
        NewBrush.Tiles_Straight.Tile_Add(33, TileDirection_Right, 1)
        NewBrush.Tiles_Corner_In.Tile_Add(34, TileDirection_BottomRight, 1)
        NewBrush.Tiles_Corner_Out.Tile_Add(35, TileDirection_BottomRight, 1)
        Painter_Urban.TransitionBrush_Add(NewBrush)
        'orange to green transition brush
        NewBrush = New clsPainter.sTransition_Brush
        NewBrush.Name = "Orange->Green"
        NewBrush.Terrain_Inner = Terrain_Orange
        NewBrush.Terrain_Outer = Terrain_Green
        NewBrush.Tiles_Straight.Tile_Add(39, TileDirection_Right, 1)
        NewBrush.Tiles_Corner_In.Tile_Add(38, TileDirection_TopLeft, 1)
        NewBrush.Tiles_Corner_Out.Tile_Add(37, TileDirection_TopLeft, 1)
        Painter_Urban.TransitionBrush_Add(NewBrush)
        'orange to gray transition brush
        NewBrush = New clsPainter.sTransition_Brush
        NewBrush.Name = "Orange->Gray"
        NewBrush.Terrain_Inner = Terrain_Orange
        NewBrush.Terrain_Outer = Terrain_Gray
        NewBrush.Tiles_Straight.Tile_Add(60, TileDirection_Right, 1)
        NewBrush.Tiles_Corner_In.Tile_Add(73, TileDirection_TopLeft, 1)
        NewBrush.Tiles_Corner_Out.Tile_Add(72, TileDirection_TopLeft, 1)
        Painter_Urban.TransitionBrush_Add(NewBrush)
        'orange to concrete transition brush
        NewBrush = New clsPainter.sTransition_Brush
        NewBrush.Name = "Orange->Concrete"
        NewBrush.Terrain_Inner = Terrain_Orange
        NewBrush.Terrain_Outer = Terrain_Concrete
        NewBrush.Tiles_Straight.Tile_Add(71, TileDirection_Right, 1)
        NewBrush.Tiles_Corner_In.Tile_Add(76, TileDirection_BottomRight, 1)
        NewBrush.Tiles_Corner_Out.Tile_Add(75, TileDirection_BottomRight, 1)
        Painter_Urban.TransitionBrush_Add(NewBrush)
        'gray to green transition brush
        NewBrush = New clsPainter.sTransition_Brush
        NewBrush.Name = "Gray->Green"
        NewBrush.Terrain_Inner = Terrain_Gray
        NewBrush.Terrain_Outer = Terrain_Green
        NewBrush.Tiles_Straight.Tile_Add(77, TileDirection_Right, 1)
        NewBrush.Tiles_Corner_In.Tile_Add(58, TileDirection_BottomLeft, 1)
        NewBrush.Tiles_Corner_Out.Tile_Add(79, TileDirection_BottomLeft, 1)
        Painter_Urban.TransitionBrush_Add(NewBrush)

        'road
        Dim Road_Road As New clsPainter.clsRoad
        Road_Road.Name = "Road"
        Painter_Urban.Road_Add(Road_Road)
        'road green
        NewRoadBrush = New clsPainter.sRoad_Brush
        NewRoadBrush.Road = Road_Road
        NewRoadBrush.Terrain = Terrain_Green
        NewRoadBrush.Tile_CrossIntersection.Tile_Add(49, TileDirection_None, 1)
        NewRoadBrush.Tile_TIntersection.Tile_Add(40, TileDirection_Bottom, 1)
        NewRoadBrush.Tile_Straight.Tile_Add(42, TileDirection_Left, 1)
        NewRoadBrush.Tile_End.Tile_Add(45, TileDirection_Left, 1)
        Painter_Urban.RoadBrush_Add(NewRoadBrush)
        'road blue
        NewRoadBrush = New clsPainter.sRoad_Brush
        NewRoadBrush.Road = Road_Road
        NewRoadBrush.Terrain = Terrain_Blue
        NewRoadBrush.Tile_CrossIntersection.Tile_Add(49, TileDirection_None, 1)
        NewRoadBrush.Tile_TIntersection.Tile_Add(40, TileDirection_Bottom, 1)
        NewRoadBrush.Tile_Straight.Tile_Add(42, TileDirection_Left, 1)
        NewRoadBrush.Tile_End.Tile_Add(41, TileDirection_Left, 1)
        Painter_Urban.RoadBrush_Add(NewRoadBrush)
        'road gray
        NewRoadBrush = New clsPainter.sRoad_Brush
        NewRoadBrush.Road = Road_Road
        NewRoadBrush.Terrain = Terrain_Gray
        NewRoadBrush.Tile_CrossIntersection.Tile_Add(49, TileDirection_None, 1)
        NewRoadBrush.Tile_TIntersection.Tile_Add(40, TileDirection_Bottom, 1)
        NewRoadBrush.Tile_Straight.Tile_Add(42, TileDirection_Left, 1)
        NewRoadBrush.Tile_End.Tile_Add(43, TileDirection_Left, 1)
        NewRoadBrush.Tile_End.Tile_Add(44, TileDirection_Left, 1)
        Painter_Urban.RoadBrush_Add(NewRoadBrush)
        'road orange
        NewRoadBrush = New clsPainter.sRoad_Brush
        NewRoadBrush.Road = Road_Road
        NewRoadBrush.Terrain = Terrain_Orange
        NewRoadBrush.Tile_CrossIntersection.Tile_Add(49, TileDirection_None, 1)
        NewRoadBrush.Tile_TIntersection.Tile_Add(40, TileDirection_Bottom, 1)
        NewRoadBrush.Tile_Straight.Tile_Add(42, TileDirection_Left, 1)
        Painter_Urban.RoadBrush_Add(NewRoadBrush)
        'road concrete
        NewRoadBrush = New clsPainter.sRoad_Brush
        NewRoadBrush.Road = Road_Road
        NewRoadBrush.Terrain = Terrain_Concrete
        NewRoadBrush.Tile_CrossIntersection.Tile_Add(49, TileDirection_None, 1)
        NewRoadBrush.Tile_TIntersection.Tile_Add(40, TileDirection_Bottom, 1)
        NewRoadBrush.Tile_Straight.Tile_Add(42, TileDirection_Left, 1)
        Painter_Urban.RoadBrush_Add(NewRoadBrush)

        With Generator_TilesetUrban
            ReDim .OldTextureLayers.Layers(-1)
            .OldTextureLayers.LayerCount = 0
        End With

        Dim NewLayer As frmMapTexturer.sLayerList.clsLayer

        NewLayer = New frmMapTexturer.sLayerList.clsLayer
        With Generator_TilesetUrban
            .OldTextureLayers.Layer_Insert(.OldTextureLayers.LayerCount, NewLayer)
        End With
        With NewLayer
            .Terrain = Terrain_Gray
            .HeightMax = 255.0F
            .SlopeMax = RadOf90Deg
            .Scale = 0.0F
            .Density = 1.0F
        End With

        NewLayer = New frmMapTexturer.sLayerList.clsLayer
        With Generator_TilesetUrban
            .OldTextureLayers.Layer_Insert(.OldTextureLayers.LayerCount, NewLayer)
        End With
        With NewLayer
            .Terrain = Terrain_Water
            .HeightMax = -1.0F
            .SlopeMax = -1.0F
            .Scale = 0.0F
            .Density = 1.0F
        End With

        NewLayer = New frmMapTexturer.sLayerList.clsLayer
        With Generator_TilesetUrban
            .OldTextureLayers.Layer_Insert(.OldTextureLayers.LayerCount, NewLayer)
        End With
        With NewLayer
            .AvoidLayers(1) = True
            .Terrain = Terrain_Blue
            .HeightMax = 255.0F
            .SlopeMax = -1.0F
            .Scale = 3.0F
            .Density = 0.3F
        End With

        NewLayer = New frmMapTexturer.sLayerList.clsLayer
        With Generator_TilesetUrban
            .OldTextureLayers.Layer_Insert(.OldTextureLayers.LayerCount, NewLayer)
        End With
        With NewLayer
            .AvoidLayers(1) = True
            .AvoidLayers(2) = True
            .Terrain = Terrain_Orange
            .HeightMax = 255.0F
            .SlopeMax = -1.0F
            .Scale = 2.5F
            .Density = 0.4F
        End With

        NewLayer = New frmMapTexturer.sLayerList.clsLayer
        With Generator_TilesetUrban
            .OldTextureLayers.Layer_Insert(.OldTextureLayers.LayerCount, NewLayer)
        End With
        With NewLayer
            .AvoidLayers(1) = True
            .AvoidLayers(2) = True
            .AvoidLayers(3) = True
            .Terrain = Terrain_Concrete
            .HeightMax = 255.0F
            .SlopeMax = -1.0F
            .Scale = 1.5F
            .Density = 0.6F
        End With

        NewLayer = New frmMapTexturer.sLayerList.clsLayer
        With Generator_TilesetUrban
            .OldTextureLayers.Layer_Insert(.OldTextureLayers.LayerCount, NewLayer)
        End With
        With NewLayer
            .AvoidLayers(1) = True
            .AvoidLayers(2) = True
            .AvoidLayers(3) = True
            .AvoidLayers(4) = True
            .Terrain = Terrain_Green
            .HeightMax = 255.0F
            .SlopeMax = -1.0F
            .Scale = 2.5F
            .Density = 0.6F
        End With

        NewLayer = New frmMapTexturer.sLayerList.clsLayer
        With Generator_TilesetUrban
            .OldTextureLayers.Layer_Insert(.OldTextureLayers.LayerCount, NewLayer)
        End With
        With NewLayer
            .WithinLayer = 2
            .Terrain = Terrain_Orange
            .HeightMax = 255.0F
            .SlopeMax = RadOf90Deg
            .Scale = 1.5F
            .Density = 0.5F
        End With

        NewLayer = New frmMapTexturer.sLayerList.clsLayer
        With Generator_TilesetUrban
            .OldTextureLayers.Layer_Insert(.OldTextureLayers.LayerCount, NewLayer)
        End With
        With NewLayer
            .WithinLayer = 3
            .Terrain = Terrain_Blue
            .HeightMax = 255.0F
            .SlopeMax = RadOf90Deg
            .Scale = 1.5F
            .Density = 0.5F
        End With

        NewLayer = New frmMapTexturer.sLayerList.clsLayer
        With Generator_TilesetUrban
            .OldTextureLayers.Layer_Insert(.OldTextureLayers.LayerCount, NewLayer)
        End With
        With NewLayer
            .WithinLayer = 3
            .AvoidLayers(7) = True
            .Terrain = Terrain_Green
            .HeightMax = 255.0F
            .SlopeMax = RadOf90Deg
            .Scale = 1.5F
            .Density = 0.5F
        End With
    End Sub

    Public Sub CreatePainterRockies()
        Dim NewBrushCliff As clsPainter.sCliff_Brush
        Dim NewBrush As clsPainter.sTransition_Brush
        Dim NewRoadBrush As clsPainter.sRoad_Brush

        Painter_Rockies = New clsPainter

        Dim Terrain_Grass As New clsPainter.clsTerrain
        Terrain_Grass.Name = "Grass"
        Painter_Rockies.Terrain_Add(Terrain_Grass)

        Dim Terrain_Gravel As New clsPainter.clsTerrain
        Terrain_Gravel.Name = "Gravel"
        Painter_Rockies.Terrain_Add(Terrain_Gravel)

        Dim Terrain_Dirt As New clsPainter.clsTerrain
        Terrain_Dirt.Name = "Dirt"
        Painter_Rockies.Terrain_Add(Terrain_Dirt)

        Dim Terrain_GrassSnow As New clsPainter.clsTerrain
        Terrain_GrassSnow.Name = "Grass Snow"
        Painter_Rockies.Terrain_Add(Terrain_GrassSnow)

        Dim Terrain_GravelSnow As New clsPainter.clsTerrain
        Terrain_GravelSnow.Name = "Gravel Snow"
        Painter_Rockies.Terrain_Add(Terrain_GravelSnow)

        Dim Terrain_Snow As New clsPainter.clsTerrain
        Terrain_Snow.Name = "Snow"
        Painter_Rockies.Terrain_Add(Terrain_Snow)

        Dim Terrain_Concrete As New clsPainter.clsTerrain
        Terrain_Concrete.Name = "Concrete"
        Painter_Rockies.Terrain_Add(Terrain_Concrete)

        Dim Terrain_Water As New clsPainter.clsTerrain
        Terrain_Water.Name = "Water"
        Painter_Rockies.Terrain_Add(Terrain_Water)

        'grass centre brush
        Terrain_Grass.Tiles.Tile_Add(0, TileDirection_None, 1)
        'gravel centre brush
        Terrain_Gravel.Tiles.Tile_Add(5, TileDirection_None, 1)
        Terrain_Gravel.Tiles.Tile_Add(6, TileDirection_None, 1)
        Terrain_Gravel.Tiles.Tile_Add(7, TileDirection_None, 1)
        'dirt centre brush
        Terrain_Dirt.Tiles.Tile_Add(53, TileDirection_None, 1)
        'grass snow centre brush
        Terrain_GrassSnow.Tiles.Tile_Add(23, TileDirection_None, 1)
        'gravel snow centre brush
        Terrain_GravelSnow.Tiles.Tile_Add(41, TileDirection_None, 1)
        'snow centre brush
        Terrain_Snow.Tiles.Tile_Add(64, TileDirection_None, 1)
        'concrete centre brush
        Terrain_Concrete.Tiles.Tile_Add(22, TileDirection_None, 1)
        'water centre brush
        Terrain_Water.Tiles.Tile_Add(17, TileDirection_None, 1)
        'gravel to gravel cliff brush
        NewBrushCliff = New clsPainter.sCliff_Brush
        NewBrushCliff.Name = "Gravel Cliff"
        NewBrushCliff.Terrain_Inner = Terrain_Gravel
        NewBrushCliff.Terrain_Outer = Terrain_Gravel
        NewBrushCliff.Tiles_Straight.Tile_Add(46, TileDirection_Bottom, 1)
        NewBrushCliff.Tiles_Straight.Tile_Add(71, TileDirection_Bottom, 1)
        NewBrushCliff.Tiles_Corner_In.Tile_Add(45, TileDirection_TopRight, 1)
        NewBrushCliff.Tiles_Corner_Out.Tile_Add(45, TileDirection_BottomLeft, 1)
        Painter_Rockies.CliffBrush_Add(NewBrushCliff)
        'gravel snow to gravel cliff brush
        NewBrushCliff = New clsPainter.sCliff_Brush
        NewBrushCliff.Name = "Gravel Snow -> Gravel Cliff"
        NewBrushCliff.Terrain_Inner = Terrain_GravelSnow
        NewBrushCliff.Terrain_Outer = Terrain_Gravel
        NewBrushCliff.Tiles_Straight.Tile_Add(29, TileDirection_Bottom, 1)
        NewBrushCliff.Tiles_Corner_In.Tile_Add(9, TileDirection_TopLeft, 1)
        NewBrushCliff.Tiles_Corner_Out.Tile_Add(42, TileDirection_BottomLeft, 1)
        Painter_Rockies.CliffBrush_Add(NewBrushCliff)
        'snow to gravel cliff brush
        NewBrushCliff = New clsPainter.sCliff_Brush
        NewBrushCliff.Name = "Snow -> Gravel Cliff"
        NewBrushCliff.Terrain_Inner = Terrain_Snow
        NewBrushCliff.Terrain_Outer = Terrain_Gravel
        NewBrushCliff.Tiles_Straight.Tile_Add(68, TileDirection_Bottom, 1)
        NewBrushCliff.Tiles_Corner_In.Tile_Add(63, TileDirection_TopLeft, 1)
        NewBrushCliff.Tiles_Corner_Out.Tile_Add(42, TileDirection_BottomLeft, 1)
        Painter_Rockies.CliffBrush_Add(NewBrushCliff)
        'gravel snow cliff brush
        NewBrushCliff = New clsPainter.sCliff_Brush
        NewBrushCliff.Name = "Gravel Snow Cliff"
        NewBrushCliff.Terrain_Inner = Terrain_GravelSnow
        NewBrushCliff.Terrain_Outer = Terrain_GravelSnow
        NewBrushCliff.Tiles_Straight.Tile_Add(44, TileDirection_Bottom, 1)
        NewBrushCliff.Tiles_Corner_In.Tile_Add(9, TileDirection_TopLeft, 1)
        NewBrushCliff.Tiles_Corner_Out.Tile_Add(9, TileDirection_BottomRight, 1)
        Painter_Rockies.CliffBrush_Add(NewBrushCliff)
        'snow to gravel snow cliff brush
        NewBrushCliff = New clsPainter.sCliff_Brush
        NewBrushCliff.Name = "Snow -> Gravel Snow Cliff"
        NewBrushCliff.Terrain_Inner = Terrain_Snow
        NewBrushCliff.Terrain_Outer = Terrain_GravelSnow
        NewBrushCliff.Tiles_Straight.Tile_Add(78, TileDirection_Bottom, 1)
        NewBrushCliff.Tiles_Corner_In.Tile_Add(63, TileDirection_TopLeft, 1)
        NewBrushCliff.Tiles_Corner_Out.Tile_Add(9, TileDirection_BottomRight, 1)
        Painter_Rockies.CliffBrush_Add(NewBrushCliff)
        'snow to snow cliff brush
        NewBrushCliff = New clsPainter.sCliff_Brush
        NewBrushCliff.Name = "Snow -> Snow Cliff"
        NewBrushCliff.Terrain_Inner = Terrain_Snow
        NewBrushCliff.Terrain_Outer = Terrain_Snow
        NewBrushCliff.Tiles_Straight.Tile_Add(78, TileDirection_Bottom, 1)
        NewBrushCliff.Tiles_Corner_In.Tile_Add(63, TileDirection_TopLeft, 1)
        NewBrushCliff.Tiles_Corner_Out.Tile_Add(63, TileDirection_BottomRight, 1)
        Painter_Rockies.CliffBrush_Add(NewBrushCliff)
        'water to grass transition brush
        NewBrush = New clsPainter.sTransition_Brush
        NewBrush.Name = "Water -> Grass"
        NewBrush.Terrain_Inner = Terrain_Water
        NewBrush.Terrain_Outer = Terrain_Grass
        NewBrush.Tiles_Straight.Tile_Add(14, TileDirection_Bottom, 1)
        NewBrush.Tiles_Corner_In.Tile_Add(16, TileDirection_BottomLeft, 1)
        NewBrush.Tiles_Corner_Out.Tile_Add(15, TileDirection_BottomLeft, 1)
        Painter_Rockies.TransitionBrush_Add(NewBrush)
        'water to gravel transition brush
        NewBrush = New clsPainter.sTransition_Brush
        NewBrush.Name = "Water -> Gravel"
        NewBrush.Terrain_Inner = Terrain_Water
        NewBrush.Terrain_Outer = Terrain_Gravel
        NewBrush.Tiles_Straight.Tile_Add(31, TileDirection_Top, 1)
        NewBrush.Tiles_Corner_In.Tile_Add(32, TileDirection_TopLeft, 1)
        NewBrush.Tiles_Corner_Out.Tile_Add(33, TileDirection_TopLeft, 1)
        Painter_Rockies.TransitionBrush_Add(NewBrush)
        'grass to gravel transition brush
        NewBrush = New clsPainter.sTransition_Brush
        NewBrush.Name = "Grass -> Gravel"
        NewBrush.Terrain_Inner = Terrain_Grass
        NewBrush.Terrain_Outer = Terrain_Gravel
        NewBrush.Tiles_Straight.Tile_Add(2, TileDirection_Left, 1)
        NewBrush.Tiles_Corner_In.Tile_Add(3, TileDirection_TopLeft, 1)
        NewBrush.Tiles_Corner_Out.Tile_Add(4, TileDirection_TopLeft, 1)
        Painter_Rockies.TransitionBrush_Add(NewBrush)
        'grass to grass snow transition brush
        NewBrush = New clsPainter.sTransition_Brush
        NewBrush.Name = "Grass -> Grass Snow"
        NewBrush.Terrain_Inner = Terrain_Grass
        NewBrush.Terrain_Outer = Terrain_GrassSnow
        NewBrush.Tiles_Straight.Tile_Add(26, TileDirection_Top, 1)
        NewBrush.Tiles_Corner_In.Tile_Add(25, TileDirection_TopLeft, 1)
        NewBrush.Tiles_Corner_Out.Tile_Add(24, TileDirection_TopLeft, 1)
        Painter_Rockies.TransitionBrush_Add(NewBrush)
        'grass to dirt transition brush
        NewBrush = New clsPainter.sTransition_Brush
        NewBrush.Name = "Grass -> Dirt"
        NewBrush.Terrain_Inner = Terrain_Grass
        NewBrush.Terrain_Outer = Terrain_Dirt
        NewBrush.Tiles_Straight.Tile_Add(34, TileDirection_Right, 1)
        NewBrush.Tiles_Corner_In.Tile_Add(35, TileDirection_BottomRight, 1)
        NewBrush.Tiles_Corner_Out.Tile_Add(36, TileDirection_BottomRight, 1)
        Painter_Rockies.TransitionBrush_Add(NewBrush)
        'gravel snow to gravel transition brush
        NewBrush = New clsPainter.sTransition_Brush
        NewBrush.Name = "Gravel Snow -> Gravel"
        NewBrush.Terrain_Inner = Terrain_GravelSnow
        NewBrush.Terrain_Outer = Terrain_Gravel
        NewBrush.Tiles_Straight.Tile_Add(12, TileDirection_Bottom, 1)
        NewBrush.Tiles_Corner_In.Tile_Add(10, TileDirection_BottomRight, 1)
        NewBrush.Tiles_Corner_Out.Tile_Add(11, TileDirection_BottomRight, 1)
        Painter_Rockies.TransitionBrush_Add(NewBrush)
        'snow to gravel snow transition brush
        NewBrush = New clsPainter.sTransition_Brush
        NewBrush.Name = "Snow -> Gravel Snow"
        NewBrush.Terrain_Inner = Terrain_Snow
        NewBrush.Terrain_Outer = Terrain_GravelSnow
        NewBrush.Tiles_Straight.Tile_Add(67, TileDirection_Bottom, 1)
        NewBrush.Tiles_Corner_In.Tile_Add(65, TileDirection_BottomRight, 1)
        NewBrush.Tiles_Corner_Out.Tile_Add(66, TileDirection_BottomRight, 1)
        Painter_Rockies.TransitionBrush_Add(NewBrush)
        'concrete to dirt transition brush
        NewBrush = New clsPainter.sTransition_Brush
        NewBrush.Name = "Concrete -> Dirt"
        NewBrush.Terrain_Inner = Terrain_Concrete
        NewBrush.Terrain_Outer = Terrain_Dirt
        NewBrush.Tiles_Straight.Tile_Add(21, TileDirection_Right, 1)
        NewBrush.Tiles_Corner_In.Tile_Add(19, TileDirection_BottomRight, 1)
        NewBrush.Tiles_Corner_Out.Tile_Add(20, TileDirection_BottomRight, 1)
        Painter_Rockies.TransitionBrush_Add(NewBrush)
        'gravel to dirt transition brush
        NewBrush = New clsPainter.sTransition_Brush
        NewBrush.Name = "Gravel -> Dirt"
        NewBrush.Terrain_Inner = Terrain_Gravel
        NewBrush.Terrain_Outer = Terrain_Dirt
        NewBrush.Tiles_Straight.Tile_Add(38, TileDirection_Left, 1)
        NewBrush.Tiles_Corner_In.Tile_Add(40, TileDirection_TopLeft, 1)
        NewBrush.Tiles_Corner_Out.Tile_Add(39, TileDirection_TopLeft, 1)
        Painter_Rockies.TransitionBrush_Add(NewBrush)
        'road
        Dim Road_Road As New clsPainter.clsRoad
        Road_Road.Name = "Road"
        Painter_Rockies.Road_Add(Road_Road)
        'road brown
        NewRoadBrush = New clsPainter.sRoad_Brush
        NewRoadBrush.Road = Road_Road
        NewRoadBrush.Terrain = Terrain_Dirt
        NewRoadBrush.Tile_TIntersection.Tile_Add(13, TileDirection_Bottom, 1)
        NewRoadBrush.Tile_Straight.Tile_Add(59, TileDirection_Left, 1)
        NewRoadBrush.Tile_End.Tile_Add(60, TileDirection_Left, 1)
        Painter_Rockies.RoadBrush_Add(NewRoadBrush)
        'track
        Dim Road_Track As New clsPainter.clsRoad
        Road_Track.Name = "Track"
        Painter_Rockies.Road_Add(Road_Track)
        'track brown
        NewRoadBrush = New clsPainter.sRoad_Brush
        NewRoadBrush.Road = Road_Track
        NewRoadBrush.Terrain = Terrain_Dirt
        NewRoadBrush.Tile_TIntersection.Tile_Add(72, TileDirection_Right, 1)
        NewRoadBrush.Tile_Straight.Tile_Add(49, TileDirection_Top, 1)
        NewRoadBrush.Tile_Straight.Tile_Add(51, TileDirection_Top, 2)
        NewRoadBrush.Tile_Corner_In.Tile_Add(50, TileDirection_BottomRight, 1)
        NewRoadBrush.Tile_End.Tile_Add(52, TileDirection_Bottom, 1)
        Painter_Rockies.RoadBrush_Add(NewRoadBrush)

        With Generator_TilesetRockies
            ReDim .OldTextureLayers.Layers(-1)
            .OldTextureLayers.LayerCount = 0
        End With

        Dim NewLayer As frmMapTexturer.sLayerList.clsLayer

        NewLayer = New frmMapTexturer.sLayerList.clsLayer
        With Generator_TilesetRockies
            .OldTextureLayers.Layer_Insert(.OldTextureLayers.LayerCount, NewLayer)
        End With
        With NewLayer
            .Terrain = Terrain_Gravel
            .HeightMax = 255.0F
            .SlopeMax = RadOf90Deg
            .Scale = 0.0F
            .Density = 1.0F
        End With

        NewLayer = New frmMapTexturer.sLayerList.clsLayer
        With Generator_TilesetRockies
            .OldTextureLayers.Layer_Insert(.OldTextureLayers.LayerCount, NewLayer)
        End With
        With NewLayer
            .Terrain = Terrain_Water
            .HeightMax = -1.0F
            .SlopeMax = -1.0F
            .Scale = 0.0F
            .Density = 1.0F
        End With

        NewLayer = New frmMapTexturer.sLayerList.clsLayer
        With Generator_TilesetRockies
            .OldTextureLayers.Layer_Insert(.OldTextureLayers.LayerCount, NewLayer)
        End With
        With NewLayer
            .AvoidLayers(1) = True
            .Terrain = Terrain_Grass
            .HeightMax = 60.0F
            .SlopeMax = -1.0F
            .Scale = 0.0F
            .Density = 1.0F
        End With

        NewLayer = New frmMapTexturer.sLayerList.clsLayer
        With Generator_TilesetRockies
            .OldTextureLayers.Layer_Insert(.OldTextureLayers.LayerCount, NewLayer)
        End With
        With NewLayer
            .AvoidLayers(1) = True
            .AvoidLayers(3) = True
            .Terrain = Terrain_GravelSnow
            .HeightMin = 150.0F
            .HeightMax = 255.0F
            .SlopeMax = RadOf90Deg
            .Scale = 0.0F
            .Density = 1.0F
        End With

        NewLayer = New frmMapTexturer.sLayerList.clsLayer
        With Generator_TilesetRockies
            .OldTextureLayers.Layer_Insert(.OldTextureLayers.LayerCount, NewLayer)
        End With
        With NewLayer
            .WithinLayer = 3
            .AvoidLayers(1) = True
            .Terrain = Terrain_Snow
            .HeightMin = 200.0F
            .HeightMax = 255.0F
            .SlopeMax = RadOf90Deg
            .Scale = 0.0F
            .Density = 1.0F
        End With

        NewLayer = New frmMapTexturer.sLayerList.clsLayer
        With Generator_TilesetRockies
            .OldTextureLayers.Layer_Insert(.OldTextureLayers.LayerCount, NewLayer)
        End With
        With NewLayer
            .WithinLayer = 3
            .AvoidLayers(4) = True
            .Terrain = Terrain_Snow
            .HeightMin = 150.0F
            .HeightMax = 255.0F
            .SlopeMax = -1.0F
            .Scale = 1.5F
            .Density = 0.45F
        End With

        NewLayer = New frmMapTexturer.sLayerList.clsLayer
        With Generator_TilesetRockies
            .OldTextureLayers.Layer_Insert(.OldTextureLayers.LayerCount, NewLayer)
        End With
        With NewLayer
            .AvoidLayers(1) = True
            .AvoidLayers(2) = True
            .AvoidLayers(3) = True
            .Terrain = Terrain_GravelSnow
            .HeightMin = 0.0F
            .HeightMax = 255.0F
            .SlopeMax = -1.0F
            .Scale = 1.5F
            .Density = 0.45F
        End With

        NewLayer = New frmMapTexturer.sLayerList.clsLayer
        With Generator_TilesetRockies
            .OldTextureLayers.Layer_Insert(.OldTextureLayers.LayerCount, NewLayer)
        End With
        With NewLayer
            .AvoidLayers(1) = True
            .WithinLayer = 2
            .Terrain = Terrain_Dirt
            .HeightMin = 0.0F
            .HeightMax = 255.0F
            .SlopeMax = RadOf90Deg
            .Scale = 1.0F
            .Density = 0.3F
        End With
    End Sub

    Public Class clsZipStreamEntry
        Public Stream As Zip.ZipInputStream
        Public Entry As Zip.ZipEntry

        Public Function BeginNewReadFile() As clsReadFile
            Dim File As New clsReadFile

            File.Begin(Stream, Entry.Size)
            Return File
        End Function
    End Class

    Public Function FindZipEntryFromPath(ByVal Path As String, ByVal ZipPathToFind As String) As clsZipStreamEntry
        Dim ZipStream As Zip.ZipInputStream
        Dim ZipEntry As Zip.ZipEntry
        Dim FindPath As String = ZipPathToFind.ToLower.Replace("\"c, "/"c)
        Dim tmpPath As String

        ZipStream = New Zip.ZipInputStream(IO.File.OpenRead(Path))
        Do
            Try
                ZipEntry = ZipStream.GetNextEntry
            Catch ex As Exception
                Exit Do
            End Try
            If ZipEntry Is Nothing Then
                Exit Do
            End If

            tmpPath = ZipEntry.Name.ToLower.Replace("\"c, "/"c)
            If tmpPath = FindPath Then
                Dim Result As New clsZipStreamEntry
                Result.Stream = ZipStream
                Result.Entry = ZipEntry
                Return Result
            End If
        Loop
        ZipStream.Close()

        Return Nothing
    End Function

    Public TemplateDroidType_Droid As clsDroidDesign.clsTemplateDroidType
    Public TemplateDroidType_Cyborg As clsDroidDesign.clsTemplateDroidType
    Public TemplateDroidType_CyborgConstruct As clsDroidDesign.clsTemplateDroidType
    Public TemplateDroidType_CyborgRepair As clsDroidDesign.clsTemplateDroidType
    Public TemplateDroidType_CyborgSuper As clsDroidDesign.clsTemplateDroidType
    Public TemplateDroidType_Transporter As clsDroidDesign.clsTemplateDroidType
    Public TemplateDroidType_Person As clsDroidDesign.clsTemplateDroidType
    Public TemplateDroidType_Null As clsDroidDesign.clsTemplateDroidType

    Public Sub CreateTemplateDroidTypes()

        TemplateDroidType_Droid = New clsDroidDesign.clsTemplateDroidType("Droid", "DROID")
        TemplateDroidType_Droid.Num = TemplateDroidType_Add(TemplateDroidType_Droid)

        TemplateDroidType_Cyborg = New clsDroidDesign.clsTemplateDroidType("Cyborg", "CYBORG")
        TemplateDroidType_Cyborg.Num = TemplateDroidType_Add(TemplateDroidType_Cyborg)

        TemplateDroidType_CyborgConstruct = New clsDroidDesign.clsTemplateDroidType("Cyborg Construct", "CYBORG_CONSTRUCT")
        TemplateDroidType_CyborgConstruct.Num = TemplateDroidType_Add(TemplateDroidType_CyborgConstruct)

        TemplateDroidType_CyborgRepair = New clsDroidDesign.clsTemplateDroidType("Cyborg Repair", "CYBORG_REPAIR")
        TemplateDroidType_CyborgRepair.Num = TemplateDroidType_Add(TemplateDroidType_CyborgRepair)

        TemplateDroidType_CyborgSuper = New clsDroidDesign.clsTemplateDroidType("Cyborg Super", "CYBORG_SUPER")
        TemplateDroidType_CyborgSuper.Num = TemplateDroidType_Add(TemplateDroidType_CyborgSuper)

        TemplateDroidType_Transporter = New clsDroidDesign.clsTemplateDroidType("Transporter", "TRANSPORTER")
        TemplateDroidType_Transporter.Num = TemplateDroidType_Add(TemplateDroidType_Transporter)

        TemplateDroidType_Person = New clsDroidDesign.clsTemplateDroidType("Person", "PERSON")
        TemplateDroidType_Person.Num = TemplateDroidType_Add(TemplateDroidType_Person)

        TemplateDroidType_Null = New clsDroidDesign.clsTemplateDroidType("Null Droid", "ZNULLDROID")
        TemplateDroidType_Null.Num = TemplateDroidType_Add(TemplateDroidType_Null)
    End Sub

    Public Function GetTemplateDroidTypeFromTemplateCode(ByVal Code As String) As clsDroidDesign.clsTemplateDroidType
        Dim LCaseCode As String = Code.ToLower
        Dim A As Integer

        For A = 0 To TemplateDroidTypeCount - 1
            If TemplateDroidTypes(A).TemplateCode.ToLower = LCaseCode Then
                Return TemplateDroidTypes(A)
            End If
        Next
        Return Nothing
    End Function

    Public Function TemplateDroidType_Add(ByVal NewDroidType As clsDroidDesign.clsTemplateDroidType) As Integer

        ReDim Preserve TemplateDroidTypes(TemplateDroidTypeCount)
        TemplateDroidTypes(TemplateDroidTypeCount) = NewDroidType
        TemplateDroidType_Add = TemplateDroidTypeCount
        TemplateDroidTypeCount += 1
    End Function

    Public Enum enumDroidType As Byte
        Weapon = 0
        Sensor = 1
        ECM = 2
        Construct = 3
        Person = 4
        Cyborg = 5
        Transporter = 6
        Command = 7
        Repair = 8
        Default_ = 9
        Cyborg_Construct = 10
        Cyborg_Repair = 11
        Cyborg_Super = 12
    End Enum

    Public Sub ShowWarnings(ByVal Result As clsResult, ByVal Title As String)

        If Not Result.HasWarnings Then
            Exit Sub
        End If

        Dim WarningsForm As New frmWarnings(Result, Title, ProgramIcon)
        WarningsForm.Show()
        WarningsForm.Activate()
    End Sub

    Public Function GetTurretFromTypeAndCode(ByVal TurretTypeName As String, ByVal TurretCode As String) As clsTurret
        Dim LCaseTypeName As String = TurretTypeName.ToLower

        Select Case LCaseTypeName
            Case "weapon"
                Return FindWeaponCode(TurretCode)
            Case "construct"
                Return FindConstructCode(TurretCode)
            Case "repair"
                Return FindRepairCode(TurretCode)
            Case "sensor"
                Return FindSensorCode(TurretCode)
            Case "brain"
                Return FindBrainCode(TurretCode)
            Case "ecm"
                Return FindECMCode(TurretCode)
            Case Else
                Return Nothing
        End Select
    End Function

    Public Function GetTurretTypeFromName(ByVal TurretTypeName As String) As clsTurret.enumTurretType

        Select Case TurretTypeName.ToLower
            Case "weapon"
                Return clsTurret.enumTurretType.Weapon
            Case "construct"
                Return clsTurret.enumTurretType.Construct
            Case "repair"
                Return clsTurret.enumTurretType.Repair
            Case "sensor"
                Return clsTurret.enumTurretType.Sensor
            Case "brain"
                Return clsTurret.enumTurretType.Brain
            Case "ecm"
                Return clsTurret.enumTurretType.ECM
            Case Else
                Return clsTurret.enumTurretType.Unknown
        End Select
    End Function

    Public Function FindOrCreateWeapon(ByVal Code As String) As clsWeapon
        Dim Result As clsWeapon

        Result = FindWeaponCode(Code)
        If Result IsNot Nothing Then
            Return Result
        End If
        Result = New clsWeapon
        Result.IsUnknown = True
        Result.Code = Code
        Return Result
    End Function

    Public Function FindOrCreateConstruct(ByVal Code As String) As clsConstruct
        Dim Result As clsConstruct

        Result = FindConstructCode(Code)
        If Result IsNot Nothing Then
            Return Result
        End If
        Result = New clsConstruct
        Result.IsUnknown = True
        Result.Code = Code
        Return Result
    End Function

    Public Function FindOrCreateRepair(ByVal Code As String) As clsRepair
        Dim Result As clsRepair

        Result = FindRepairCode(Code)
        If Result IsNot Nothing Then
            Return Result
        End If
        Result = New clsRepair
        Result.IsUnknown = True
        Result.Code = Code
        Return Result
    End Function

    Public Function FindOrCreateSensor(ByVal Code As String) As clsSensor
        Dim Result As clsSensor

        Result = FindSensorCode(Code)
        If Result IsNot Nothing Then
            Return Result
        End If
        Result = New clsSensor
        Result.IsUnknown = True
        Result.Code = Code
        Return Result
    End Function

    Public Function FindOrCreateBrain(ByVal Code As String) As clsBrain
        Dim Result As clsBrain

        Result = FindBrainCode(Code)
        If Result IsNot Nothing Then
            Return Result
        End If
        Result = New clsBrain
        Result.IsUnknown = True
        Result.Code = Code
        Return Result
    End Function

    Public Function FindOrCreateECM(ByVal Code As String) As clsECM
        Dim Result As clsECM

        Result = FindECMCode(Code)
        If Result IsNot Nothing Then
            Return Result
        End If
        Result = New clsECM
        Result.IsUnknown = True
        Result.Code = Code
        Return Result
    End Function

    Public Function FindOrCreateTurret(ByVal TurretType As clsTurret.enumTurretType, ByVal TurretCode As String) As clsTurret

        Select Case TurretType
            Case clsTurret.enumTurretType.Weapon
                Return FindOrCreateWeapon(TurretCode)
            Case clsTurret.enumTurretType.Construct
                Return FindOrCreateConstruct(TurretCode)
            Case clsTurret.enumTurretType.Repair
                Return FindOrCreateRepair(TurretCode)
            Case clsTurret.enumTurretType.Sensor
                Return FindOrCreateSensor(TurretCode)
            Case clsTurret.enumTurretType.Brain
                Return FindOrCreateBrain(TurretCode)
            Case clsTurret.enumTurretType.ECM
                Return FindOrCreateECM(TurretCode)
            Case Else
                Return Nothing
        End Select
    End Function

    Public Function FindOrCreateBody(ByVal Code As String) As clsBody
        Dim tmpBody As clsBody

        tmpBody = FindBodyCode(Code)
        If tmpBody IsNot Nothing Then
            Return tmpBody
        End If
        tmpBody = New clsBody
        tmpBody.IsUnknown = True
        tmpBody.Code = Code
        Return tmpBody
    End Function

    Public Function FindOrCreatePropulsion(ByVal Code As String) As clsPropulsion
        Dim tmpPropulsion As clsPropulsion

        tmpPropulsion = FindPropulsionCode(Code)
        If tmpPropulsion IsNot Nothing Then
            Return tmpPropulsion
        End If
        tmpPropulsion = New clsPropulsion(BodyCount)
        tmpPropulsion.IsUnknown = True
        tmpPropulsion.Code = Code
        Return tmpPropulsion
    End Function

    Public Function FindOrCreateUnitType(ByVal Code As String, ByVal Type As clsUnitType.enumType) As clsUnitType
        Dim A As Integer

        Select Case Type
            Case clsUnitType.enumType.Feature
                For A = 0 To UnitTypeCount - 1
                    If UnitTypes(A).Type = clsUnitType.enumType.Feature Then
                        If CType(UnitTypes(A), clsFeatureType).Code = Code Then
                            Return UnitTypes(A)
                        End If
                    End If
                Next
                Dim tmpFeatureType As New clsFeatureType
                tmpFeatureType.IsUnknown = True
                tmpFeatureType.Code = Code
                tmpFeatureType.Footprint.X = 1
                tmpFeatureType.Footprint.Y = 1
                Return tmpFeatureType
            Case clsUnitType.enumType.PlayerStructure
                For A = 0 To UnitTypeCount - 1
                    If UnitTypes(A).Type = clsUnitType.enumType.PlayerStructure Then
                        If CType(UnitTypes(A), clsStructureType).Code = Code Then
                            Return UnitTypes(A)
                        End If
                    End If
                Next
                Dim tmpStructureType As New clsStructureType
                tmpStructureType.IsUnknown = True
                tmpStructureType.Code = Code
                tmpStructureType.Footprint.X = 1
                tmpStructureType.Footprint.Y = 1
                Return tmpStructureType
            Case clsUnitType.enumType.PlayerDroid
                Dim tmpDroidType As clsDroidDesign
                For A = 0 To UnitTypeCount - 1
                    If UnitTypes(A).Type = clsUnitType.enumType.PlayerDroid Then
                        tmpDroidType = CType(UnitTypes(A), clsDroidDesign)
                        If tmpDroidType.IsTemplate Then
                            If CType(tmpDroidType, clsDroidTemplate).Code = Code Then
                                Return UnitTypes(A)
                            End If
                        End If
                    End If
                Next
                Dim tmpDroidTemplate As New clsDroidTemplate
                tmpDroidTemplate.IsUnknown = True
                tmpDroidTemplate.Code = Code
                Return tmpDroidTemplate
            Case Else
                Return Nothing
        End Select
    End Function

    Public ShowIDErrorMessage As Boolean = True

    Public Sub ErrorIDChange(ByVal IntendedID As UInteger, ByVal IDUnit As clsMap.clsUnit, ByVal NameOfErrorSource As String)

        If Not ShowIDErrorMessage Then
            Exit Sub
        End If

        If IDUnit.ID = IntendedID Then
            Exit Sub
        End If

        Dim MessageText As String

        MessageText = "An object's ID has been changed unexpectedly. The error was in " & ControlChars.Quote & NameOfErrorSource & ControlChars.Quote & "." & vbCrLf & vbCrLf & "The object is of type " & IDUnit.Type.GetDisplayText & " and is at map position " & IDUnit.GetPosText & ". It's ID was " & IntendedID & ", but is now " & IDUnit.ID & "." & vbCrLf & vbCrLf & "Click Cancel to stop seeing this message. Otherwise, click OK."

        If MsgBox(MessageText, vbOKCancel) = MsgBoxResult.Cancel Then
            ShowIDErrorMessage = False
        End If
    End Sub

    Public Const ZeroResetID As UInteger = 1000000UI
    Public Const LoadModulesID As UInteger = ZeroResetID

    Public Sub ZeroIDWarning(ByVal IDUnit As clsMap.clsUnit)
        Dim MessageText As String

        MessageText = "An object's ID has been changed from 0 to " & ZeroResetID & ". Zero is not a valid ID. The object is of type " & IDUnit.Type.GetDisplayText & " and is at map position " & IDUnit.GetPosText & "."

        MsgBox(MessageText, vbOKOnly)
    End Sub

    Public Structure sWorldPos
        Public Horizontal As sXY_int
        Public Altitude As Integer

        Public Sub New(ByVal NewHorizontal As sXY_int, ByVal NewAltitude As Integer)

            Horizontal = NewHorizontal
            Altitude = NewAltitude
        End Sub
    End Structure

    Public Class clsWorldPos
        Public WorldPos As sWorldPos

        Public Sub New(ByVal NewWorldPos As sWorldPos)

            WorldPos = NewWorldPos
        End Sub
    End Class

    Public Function PosIsWithinTileArea(ByVal WorldHorizontal As sXY_int, ByVal StartTile As sXY_int, ByVal FinishTile As sXY_int) As Boolean

        Return (WorldHorizontal.X >= StartTile.X * TerrainGridSpacing And _
            WorldHorizontal.Y >= StartTile.Y * TerrainGridSpacing And _
            WorldHorizontal.X < FinishTile.X * TerrainGridSpacing And _
            WorldHorizontal.Y < FinishTile.Y * TerrainGridSpacing)
    End Function

    Public Function SizeIsPowerOf2(ByVal Size As Integer) As Boolean

        Dim Power As Double = Math.Log(Size) / Math.Log(2.0#)
        Return (Power = CInt(Power))
    End Function

    Public Function FindFirstStructureType(ByVal FindType As clsStructureType.enumStructureType) As clsStructureType
        Dim A As Integer
        Dim tmpObjectType As clsUnitType
        Dim tmpStructureType As clsStructureType

        For A = 0 To UnitTypeCount - 1
            tmpObjectType = UnitTypes(A)
            If tmpObjectType.Type = clsUnitType.enumType.PlayerStructure Then
                tmpStructureType = CType(tmpObjectType, clsStructureType)
                If tmpStructureType.StructureType = FindType Then
                    Return tmpStructureType
                End If
            End If
        Next

        Return Nothing
    End Function
End Module