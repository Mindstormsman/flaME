Imports OpenTK.Graphics.OpenGL

Public Class clsObjectData

    Public UnitTypes As New ConnectedList(Of clsUnitType, clsObjectData)(Me)

    Public FeatureTypes As New ConnectedList(Of clsFeatureType, clsObjectData)(Me)
    Public StructureTypes As New ConnectedList(Of clsStructureType, clsObjectData)(Me)
    Public DroidTemplates As New ConnectedList(Of clsDroidTemplate, clsObjectData)(Me)

    Public WallTypes As New ConnectedList(Of clsWallType, clsObjectData)(Me)

    Public Bodies As New ConnectedList(Of clsBody, clsObjectData)(Me)
    Public Propulsions As New ConnectedList(Of clsPropulsion, clsObjectData)(Me)
    Public Turrets As New ConnectedList(Of clsTurret, clsObjectData)(Me)
    Public Weapons As New ConnectedList(Of clsWeapon, clsObjectData)(Me)
    Public Sensors As New ConnectedList(Of clsSensor, clsObjectData)(Me)
    Public Repairs As New ConnectedList(Of clsRepair, clsObjectData)(Me)
    Public Constructors As New ConnectedList(Of clsConstruct, clsObjectData)(Me)
    Public Brains As New ConnectedList(Of clsBrain, clsObjectData)(Me)
    Public ECMs As New ConnectedList(Of clsECM, clsObjectData)(Me)

    Public Class clsTexturePage
        Public FileTitle As String
        Public GLTexture_Num As Integer
    End Class
    Public TexturePages As New SimpleList(Of clsTexturePage)

    Public Class clsPIE
        Public Path As String
        Public LCaseFileTitle As String
        Public Model As clsModel
    End Class

    Public Class clsTextFile
        Public SubDirectory As String
        Public FieldCount As Integer = 0
        Public UniqueField As Integer = 0

        Public ResultData As New SimpleList(Of String())

        'Find the Value between Two Delimiters after Keyword in Entry
        Public Function FindInString(Entry As String, Keyword As String, Delimiter1 As String, Delimiter2 As String) As String
            Dim Start As Integer
            Dim Finish As Integer
            Dim Key As Integer
            Dim Value As String
            Dim Length As Integer

            Key = InStr(Entry, Keyword)
            If Key = 0 Then
                Return "Default"
            End If
            Start = InStr(Key + Len(Keyword), Entry, Delimiter1)
            Finish = InStr(Start + 1, Entry, Delimiter2)
            If Finish = 0 Then
                Finish = InStr(Start + 1, Entry, "}")
            End If
            Length = Finish - Start - 1
            Value = Mid(Entry, Start + 1, Length)
            Value = Replace(Value, "}", "")
            Value = Value.Trim

            Return Value
        End Function

        Public Function FindBodyProps(Entry As String, Keyword As String, Delimiter1 As String, Delimiter2 As String) As String
            Dim Start As Integer
            Dim Finish As Integer
            Dim Key As Integer
            Dim Value As String
            Dim Length As Integer

            Key = InStr(Entry, Keyword)
            If Key = 0 Then
                Return "Default"
            End If
            Start = InStr(Key + Len(Keyword), Entry, Delimiter1)
            Finish = InStrRev(Entry, Delimiter2)
            Finish = InStrRev(Entry, Delimiter2, Finish - 1)
            Length = Finish - Start - 1
            Value = Mid(Entry, Start + 1, Length)
            Value = Replace(Value, ControlChars.Quote, "")
            Value = Replace(Value, " ", "")
            Value = Value.Trim

            Return Value
        End Function

        Public Function LoadJsonFile(Path As String) As clsResult
            Dim Result As New clsResult("Loading Json file " & ControlChars.Quote & SubDirectory & ControlChars.Quote)
            Dim Reader As IO.StreamReader

            Try
                Reader = New IO.StreamReader(Path & SubDirectory, UTF8Encoding)
            Catch ex As Exception
                Result.ProblemAdd(ex.Message)
                Return Result
            End Try

            Dim Entry As String = ""
            Dim Line As String
            Dim Layer As Integer = 0

            Do Until Reader.EndOfStream
                Line = Reader.ReadLine 'get a line
                Line = Line.Trim
                Entry &= Line 'add it to Entry
                If InStr(Line, "{") > 0 Then
                    Layer += 1
                End If
                If InStr(Line, "}") > 0 Then
                    Layer -= 1
                    If Layer = 1 Then 'If we closed a nest and are now on Layer 1 then we know we reached the end of the entry
                        'data collected'
                        'find values and clean data'
                        'everything has an ID, make that 0th value
                        Dim Id As String = FindInString(Entry, "", ControlChars.Quote, ControlChars.Quote)
                        If Id = "Default" Then
                            Result.ProblemAdd("Something very bad happened while reading IDs from Json! No ID in Entry")
                            Return Result
                        End If

                        'Get name from Json as 1st value, default to ID if name is missing
                        Dim Name As String = FindInString(Entry, ControlChars.Quote & "name" & ControlChars.Quote, ControlChars.Quote, ControlChars.Quote)
                        If Name = "Default" Then
                            Name = Id
                        End If

                        'lots of things have designable, make that 2nd value, default to 0?
                        Dim Designable As String = FindInString(Entry, ControlChars.Quote & "designable" & ControlChars.Quote, ":", ",")
                        If Designable = "Default" Then
                            Designable = "0"
                        End If

                        'hitpoints 3rd
                        Dim Hitpoints As String = FindInString(Entry, ControlChars.Quote & "hitpoints" & ControlChars.Quote, ":", ",")
                        If Hitpoints = "Default" Then
                            Hitpoints = "0"
                        End If

                        '1 or 2 used for most things: Model, SensorModel, MountModel make 4th-6th values
                        Dim Model As String = FindInString(Entry, ControlChars.Quote & "model" & ControlChars.Quote, ControlChars.Quote, ControlChars.Quote)
                        If Model = "Default" Then
                            Model = "0"
                        End If

                        Dim SensorModel As String = FindInString(Entry, ControlChars.Quote & "sensorModel" & ControlChars.Quote, ControlChars.Quote, ControlChars.Quote)
                        If SensorModel = "Default" Then
                            SensorModel = "0"
                        End If

                        Dim MountModel As String = FindInString(Entry, ControlChars.Quote & "mountModel" & ControlChars.Quote, ControlChars.Quote, ControlChars.Quote)
                        If MountModel = "Default" Then
                            MountModel = "0"
                        End If

                        'Turret is used by Brains to associate them with a weapon, 7th
                        Dim Turret As String = FindInString(Entry, ControlChars.Quote & "turret" & ControlChars.Quote, ControlChars.Quote, ControlChars.Quote)
                        If Turret = "Default" Then
                            Turret = "0"
                        End If

                        'Location is used by Sensors, 8th
                        Dim Location As String = FindInString(Entry, ControlChars.Quote & "location" & ControlChars.Quote, ControlChars.Quote, ControlChars.Quote)
                        If Location = "Default" Then
                            Location = "0"
                        End If

                        'Type 9th
                        Dim Type As String = FindInString(Entry, ControlChars.Quote & "type" & ControlChars.Quote, ControlChars.Quote, ControlChars.Quote)
                        If Type = "Default" Then
                            Type = "0"
                        End If

                        'Width 10th
                        Dim Width As String = FindInString(Entry, ControlChars.Quote & "width" & ControlChars.Quote, ":", ",")
                        If Width = "Default" Then
                            Width = "0"
                        End If

                        'Breadth 11th
                        Dim Breadth As String = FindInString(Entry, ControlChars.Quote & "breadth" & ControlChars.Quote, ":", ",")
                        If Breadth = "Default" Then
                            Breadth = "0"
                        End If

                        'BaseModel 12th
                        Dim BaseModel As String = FindInString(Entry, ControlChars.Quote & "baseModel" & ControlChars.Quote, ControlChars.Quote, ControlChars.Quote)
                        If BaseModel = "Default" Then
                            BaseModel = "0"
                        End If

                        'ECMId 13th
                        Dim ECMId As String = FindInString(Entry, ControlChars.Quote & "ecmID" & ControlChars.Quote, ControlChars.Quote, ControlChars.Quote)
                        If ECMId = "Default" Then
                            ECMId = "0"
                        End If

                        'SensorId 14th
                        Dim SensorId As String = FindInString(Entry, ControlChars.Quote & "sensorID" & ControlChars.Quote, ControlChars.Quote, ControlChars.Quote)
                        If SensorId = "Default" Then
                            SensorId = "0"
                        End If

                        'Body 15th
                        Dim Body As String = FindInString(Entry, ControlChars.Quote & "body" & ControlChars.Quote, ControlChars.Quote, ControlChars.Quote)
                        If Body = "Default" Then
                            Body = "0"
                        End If

                        'Brain 16th
                        Dim Brain As String = FindInString(Entry, ControlChars.Quote & "brain" & ControlChars.Quote, ControlChars.Quote, ControlChars.Quote)
                        If Brain = "Default" Then
                            Brain = "0"
                        End If

                        'Construct 17th
                        Dim Construct As String = FindInString(Entry, ControlChars.Quote & "construct" & ControlChars.Quote, ControlChars.Quote, ControlChars.Quote)
                        If Construct = "Default" Then
                            Construct = "0"
                        End If

                        'Repair 18th
                        Dim Repair As String = FindInString(Entry, ControlChars.Quote & "repair" & ControlChars.Quote, ControlChars.Quote, ControlChars.Quote)
                        If Repair = "Default" Then
                            Repair = "0"
                        End If

                        'Sensor 19th
                        Dim Sensor As String = FindInString(Entry, ControlChars.Quote & "sensor" & ControlChars.Quote, ControlChars.Quote, ControlChars.Quote)
                        If Sensor = "Default" Then
                            Sensor = "0"
                        End If

                        'Propulsion 20th
                        Dim Propulsion As String = FindInString(Entry, ControlChars.Quote & "propulsion" & ControlChars.Quote, ControlChars.Quote, ControlChars.Quote)
                        If Propulsion = "Default" Then
                            Propulsion = "0"
                        End If

                        'Weapons 21st
                        Dim Weapons As String = FindInString(Entry, ControlChars.Quote & "weapons" & ControlChars.Quote, "[", "]")
                        Weapons = Replace(Weapons, ControlChars.Quote, "")
                        If Weapons = "Default" Then
                            Weapons = "0"
                        End If

                        'StructureModels 22nd
                        Dim StructureModels As String = FindInString(Entry, ControlChars.Quote & "structureModel" & ControlChars.Quote, "[", "]")
                        StructureModels = Replace(StructureModels, ControlChars.Quote, "")
                        If StructureModels = "Default" Then
                            StructureModels = "0"
                        End If

                        'PropulsionArray 23rd
                        Dim PropulsionArray As String = FindBodyProps(Entry, ControlChars.Quote & "propulsionExtraModels" & ControlChars.Quote, "{", "}")
                        If PropulsionArray = "Default" Then
                            PropulsionArray = "0"
                        End If

                        'need to read left and right propulsions from Body.json in some way to replace bodypropulsionimd.txt

                        Dim Results() As String = {Id, Name, Designable, Hitpoints, Model, SensorModel, MountModel, Turret, Location,
                            Type, Width, Breadth, BaseModel, ECMId, SensorId, Body, Brain, Construct, Repair, Sensor, Propulsion,
                            Weapons, StructureModels, PropulsionArray}
                        ResultData.Add(Results)
                        'Debug line to see output
                        'Result.WarningAdd("Loaded " & Id)
                        Entry = ""
                    End If
                End If
            Loop

            Reader.Close()

            Return Result
        End Function
    End Class

    Private Structure BodyProp
        Public LeftPIE As String
        Public RightPIE As String
    End Structure

    Public Function LoadDirectory(Path As String) As clsResult
        Dim ReturnResult As New clsResult("Loading object data from " & ControlChars.Quote & Path & ControlChars.Quote)
        'ReturnResult.WarningAdd("Test")

        Path = EndWithPathSeperator(Path)

        Dim SubDirStructures As String
        Dim SubDirBrain As String
        Dim SubDirBody As String
        Dim SubDirPropulsion As String
        Dim SubDirConstruction As String
        Dim SubDirSensor As String
        Dim SubDirRepair As String
        Dim SubDirTemplates As String
        Dim SubDirWeapons As String
        Dim SubDirECM As String
        Dim SubDirFeatures As String
        Dim SubDirStructurePIE As String
        Dim SubDirBodiesPIE As String
        Dim SubDirPropPIE As String
        Dim SubDirWeaponsPIE As String
        Dim SubDirTexpages As String
        Dim SubDirFeaturePIE As String
        Dim SubDirPIEs As String

        SubDirStructures = "stats" & PlatformPathSeparator & "structure.json"
        SubDirBrain = "stats" & PlatformPathSeparator & "brain.json"
        SubDirBody = "stats" & PlatformPathSeparator & "body.json"
        SubDirPropulsion = "stats" & PlatformPathSeparator & "propulsion.json"
        SubDirConstruction = "stats" & PlatformPathSeparator & "construction.json"
        SubDirSensor = "stats" & PlatformPathSeparator & "sensor.json"
        SubDirRepair = "stats" & PlatformPathSeparator & "repair.json"
        SubDirTemplates = "stats" & PlatformPathSeparator & "templates.json"
        SubDirWeapons = "stats" & PlatformPathSeparator & "weapons.json"
        SubDirECM = "stats" & PlatformPathSeparator & "ecm.json"
        SubDirFeatures = "stats" & PlatformPathSeparator & "features.json"
        SubDirPIEs = "pies" & PlatformPathSeparator
        'SubDirStructurePIE = "structs" & PlatformPathSeparator
        SubDirStructurePIE = SubDirPIEs
        'SubDirBodiesPIE = "components" & PlatformPathSeparator & "bodies" & PlatformPathSeparator
        SubDirBodiesPIE = SubDirPIEs
        'SubDirPropPIE = "components" & PlatformPathSeparator & "prop" & PlatformPathSeparator
        SubDirPropPIE = SubDirPIEs
        'SubDirWeaponsPIE = "components" & PlatformPathSeparator & "weapons" & PlatformPathSeparator
        SubDirWeaponsPIE = SubDirPIEs
        SubDirTexpages = "texpages" & PlatformPathSeparator
        'SubDirFeaturePIE = "features" & PlatformPathSeparator
        SubDirFeaturePIE = SubDirPIEs



        Dim JsonFiles As New SimpleList(Of clsTextFile)

        Dim DataStructures As New clsTextFile
        DataStructures.SubDirectory = SubDirStructures
        JsonFiles.Add(DataStructures)

        Dim DataBrain As New clsTextFile
        DataBrain.SubDirectory = SubDirBrain
        JsonFiles.Add(DataBrain)

        Dim DataBody As New clsTextFile
        DataBody.SubDirectory = SubDirBody
        JsonFiles.Add(DataBody)

        Dim DataPropulsion As New clsTextFile
        DataPropulsion.SubDirectory = SubDirPropulsion
        JsonFiles.Add(DataPropulsion)

        Dim DataConstruction As New clsTextFile
        DataConstruction.SubDirectory = SubDirConstruction
        JsonFiles.Add(DataConstruction)

        Dim DataSensor As New clsTextFile
        DataSensor.SubDirectory = SubDirSensor
        JsonFiles.Add(DataSensor)

        Dim DataRepair As New clsTextFile
        DataRepair.SubDirectory = SubDirRepair
        JsonFiles.Add(DataRepair)

        Dim DataTemplates As New clsTextFile
        DataTemplates.SubDirectory = SubDirTemplates
        JsonFiles.Add(DataTemplates)

        Dim DataECM As New clsTextFile
        DataECM.SubDirectory = SubDirECM
        JsonFiles.Add(DataECM)

        Dim DataFeatures As New clsTextFile
        DataFeatures.SubDirectory = SubDirFeatures
        JsonFiles.Add(DataFeatures)

        Dim DataWeapons As New clsTextFile
        DataWeapons.SubDirectory = SubDirWeapons
        JsonFiles.Add(DataWeapons)

        Dim TextFile As clsTextFile

        'Load Json Files
        For Each TextFile In JsonFiles
            Dim Result As clsResult = TextFile.LoadJsonFile(Path)
            ReturnResult.Add(Result)
        Next

        If ReturnResult.HasProblems Then
            Return ReturnResult
        End If

        'load texpages

        Dim TexFiles() As String

        Try
            TexFiles = IO.Directory.GetFiles(Path & SubDirTexpages)
        Catch ex As Exception
            ReturnResult.WarningAdd("Unable to access texture pages.")
            ReDim TexFiles(-1)
        End Try

        Dim Text As String
        Dim Bitmap As Bitmap = Nothing
        Dim InstrPos2 As Integer
        Dim BitmapTextureArgs As sBitmapGLTexture
        Dim BitmapResult As sResult

        For Each Text In TexFiles
            If Right(Text, 4).ToLower = ".png" Then
                Dim Result As New clsResult("Loading texture page " & ControlChars.Quote & Text & ControlChars.Quote)
                If IO.File.Exists(Text) Then
                    BitmapResult = LoadBitmap(Text, Bitmap)
                    Dim NewPage As New clsTexturePage
                    If BitmapResult.Success Then
                        Result.Take(BitmapIsGLCompatible(Bitmap))
                        BitmapTextureArgs.MagFilter = TextureMagFilter.Nearest
                        BitmapTextureArgs.MinFilter = TextureMinFilter.Nearest
                        BitmapTextureArgs.TextureNum = 0
                        BitmapTextureArgs.MipMapLevel = 0
                        BitmapTextureArgs.Texture = Bitmap
                        BitmapTextureArgs.Perform()
                        NewPage.GLTexture_Num = BitmapTextureArgs.TextureNum
                    Else
                        Result.WarningAdd(BitmapResult.Problem)
                    End If
                    InstrPos2 = InStrRev(Text, PlatformPathSeparator)
                    NewPage.FileTitle = Strings.Mid(Text, InstrPos2 + 1, Text.Length - 4 - InstrPos2)
                    TexturePages.Add(NewPage)
                Else
                    Result.WarningAdd("Texture page missing (" & Text & ").")
                End If
                ReturnResult.Add(Result)
            End If
        Next

        'load PIEs

        Dim PIE_Files() As String
        Dim PIE_List As New SimpleList(Of clsPIE)
        Dim NewPIE As clsPIE

        Try
            PIE_Files = IO.Directory.GetFiles(Path & SubDirPIEs)
        Catch ex As Exception
            ReturnResult.WarningAdd("Unable to access PIE files.")
            ReDim PIE_Files(-1)
        End Try

        Dim SplitPath As sSplitPath

        For Each Text In PIE_Files
            SplitPath = New sSplitPath(Text)
            If SplitPath.FileExtension.ToLower = "pie" Then
                NewPIE = New clsPIE
                NewPIE.Path = Text
                NewPIE.LCaseFileTitle = SplitPath.FileTitle.ToLower
                PIE_List.Add(NewPIE)
            End If
        Next

        'interpret stats

        Dim Attachment As clsUnitType.clsAttachment
        Dim BaseAttachment As clsUnitType.clsAttachment
        Dim Connector As sXYZ_sng
        Dim StructureType As clsStructureType
        Dim FeatureType As clsFeatureType
        Dim Template As clsDroidTemplate
        Dim Body As clsBody
        Dim Propulsion As clsPropulsion
        Dim Construct As clsConstruct
        Dim Weapon As clsWeapon
        Dim Repair As clsRepair
        Dim Sensor As clsSensor
        Dim Brain As clsBrain
        Dim ECM As clsECM
        Dim Fields() As String

        'interpret body

        For Each Fields In DataBody.ResultData
            Body = New clsBody
            Body.ObjectDataLink.Connect(Bodies)
            Body.Code = Fields(0)
            Body.Name = Fields(1)
            InvariantParse_int(Fields(3), Body.Hitpoints)
            Body.Designable = (Fields(2) <> "0")
            Body.Attachment.Models.Add(GetModelForPIE(PIE_List, Fields(4).ToLower, ReturnResult)) 'Model
        Next

        'interpret propulsion

        For Each Fields In DataPropulsion.ResultData
            Propulsion = New clsPropulsion(Bodies.Count)
            Propulsion.ObjectDataLink.Connect(Propulsions)
            Propulsion.Code = Fields(0)
            Propulsion.Name = Fields(1)
            InvariantParse_int(Fields(3), Propulsion.HitPoints)
            '.Propulsions(Propulsion_Num).PIE = LCase(DataPropulsion.Entries(Propulsion_Num).FieldValues(8))
            Propulsion.Designable = (Fields(2) <> "0")
        Next

        'interpret body-propulsions

        Dim BodyPropulsionPIEs(Bodies.Count - 1, Propulsions.Count - 1) As BodyProp 'BodyPropulsions.txt is part of Body.Json, Gonna have to rewrite this...
        For A As Integer = 0 To Bodies.Count - 1
            For B As Integer = 0 To Propulsions.Count - 1
                BodyPropulsionPIEs(A, B) = New BodyProp
                BodyPropulsionPIEs(A, B).LeftPIE = "0"
                BodyPropulsionPIEs(A, B).RightPIE = "0"
            Next
        Next

        For Each Fields In DataBody.ResultData
            Body = FindBodyCode(Fields(0))

            If Fields(23) <> "0" Then
                Dim Temp1() As String = Fields(23).Split("}"c)   'Seperate Propulsions

                For A As Integer = 0 To Temp1.GetUpperBound(0) - 1

                    Dim Temp2() As String = Temp1(A).Split("{"c) 'Isolate Propulsion
                    Temp2(0) = Replace(Temp2(0), ":", "")       'Remove : from Propulsion
                    Temp2(0) = Replace(Temp2(0), ",", "")       'Remove , from Propulsion

                    Propulsion = FindPropulsionCode(Temp2(0))

                    Dim Temp3() As String = Temp2(1).Split(","c) 'Seperate Sides/Types

                    For B As Integer = 0 To Temp3.GetUpperBound(0)

                        Dim Temp4() As String = Temp3(B).Split(":"c) 'Seperate Side and Model

                        Select Case Temp4(0)
                            Case "left"
                                BodyPropulsionPIEs(Body.ObjectDataLink.ArrayPosition, Propulsion.ObjectDataLink.ArrayPosition).LeftPIE = Temp4(1).ToLower
                            Case "right"
                                BodyPropulsionPIEs(Body.ObjectDataLink.ArrayPosition, Propulsion.ObjectDataLink.ArrayPosition).RightPIE = Temp4(1).ToLower
                            Case "moving"
                            Case Else
                                ReturnResult.WarningAdd("Model type was not left right or moving during Body-Prop association for " & Fields(0) & " and " & Temp2(0))
                        End Select
                    Next
                Next
            End If
        Next

        'set propulsion-body PIEs

        For A As Integer = 0 To Propulsions.Count - 1
            Propulsion = Propulsions(A)
            For B As Integer = 0 To Bodies.Count - 1
                Body = Bodies(B)
                Propulsion.Bodies(B).LeftAttachment = New clsUnitType.clsAttachment
                Propulsion.Bodies(B).LeftAttachment.Models.Add(GetModelForPIE(PIE_List, BodyPropulsionPIEs(B, A).LeftPIE, ReturnResult))
                Propulsion.Bodies(B).RightAttachment = New clsUnitType.clsAttachment
                Propulsion.Bodies(B).RightAttachment.Models.Add(GetModelForPIE(PIE_List, BodyPropulsionPIEs(B, A).RightPIE, ReturnResult))
            Next
        Next

        'interpret construction

        For Each Fields In DataConstruction.ResultData
            Construct = New clsConstruct
            Construct.ObjectDataLink.Connect(Constructors)
            Construct.TurretObjectDataLink.Connect(Turrets)
            Construct.Code = Fields(0)
            Construct.Name = Fields(1)
            Construct.Designable = (Fields(2) <> "0")
            Construct.Attachment.Models.Add(GetModelForPIE(PIE_List, Fields(5).ToLower, ReturnResult)) 'SensorModel
        Next

        'interpret weapons

        For Each Fields In DataWeapons.ResultData
            Weapon = New clsWeapon
            Weapon.ObjectDataLink.Connect(Weapons)
            Weapon.TurretObjectDataLink.Connect(Turrets)
            Weapon.Code = Fields(0)
            Weapon.Name = Fields(1)
            InvariantParse_int(Fields(3), Weapon.HitPoints)
            Weapon.Designable = (Fields(2) <> "0")
            Weapon.Attachment.Models.Add(GetModelForPIE(PIE_List, Fields(4).ToLower, ReturnResult)) 'Model
            Weapon.Attachment.Models.Add(GetModelForPIE(PIE_List, Fields(6).ToLower, ReturnResult)) 'MountModel
        Next

        'interpret sensor

        For Each Fields In DataSensor.ResultData
            Sensor = New clsSensor
            Sensor.ObjectDataLink.Connect(Sensors)
            Sensor.TurretObjectDataLink.Connect(Turrets)
            Sensor.Code = Fields(0)
            Sensor.Name = Fields(1)
            InvariantParse_int(Fields(3), Sensor.HitPoints)
            Sensor.Designable = (Fields(2) <> "0")
            Select Case Fields(8).ToLower 'Loction
                Case "turret"
                    Sensor.Location = clsSensor.enumLocation.Turret
                Case "default"
                    Sensor.Location = clsSensor.enumLocation.Invisible
                Case Else
                    Sensor.Location = clsSensor.enumLocation.Invisible
            End Select
            Sensor.Attachment.Models.Add(GetModelForPIE(PIE_List, Fields(5).ToLower, ReturnResult)) 'SensorModel
            Sensor.Attachment.Models.Add(GetModelForPIE(PIE_List, Fields(6).ToLower, ReturnResult)) 'MountModel
        Next

        'interpret repair

        For Each Fields In DataRepair.ResultData
            Repair = New clsRepair
            Repair.ObjectDataLink.Connect(Repairs)
            Repair.TurretObjectDataLink.Connect(Turrets)
            Repair.Code = Fields(0)
            Repair.Name = Fields(1)
            Repair.Designable = (Fields(2) <> "0")
            Repair.Attachment.Models.Add(GetModelForPIE(PIE_List, Fields(4).ToLower, ReturnResult)) 'Model
            Repair.Attachment.Models.Add(GetModelForPIE(PIE_List, Fields(6).ToLower, ReturnResult)) 'MountModel
        Next

        'interpret brain

        For Each Fields In DataBrain.ResultData
            Brain = New clsBrain
            Brain.ObjectDataLink.Connect(Brains)
            Brain.TurretObjectDataLink.Connect(Turrets)
            Brain.Code = Fields(0)
            Brain.Name = Fields(1)
            Brain.Designable = True
            Weapon = FindWeaponCode(Fields(7)) 'Turret
            If Weapon IsNot Nothing Then
                Brain.Weapon = Weapon
                Brain.Attachment = Weapon.Attachment
            End If
        Next

        'interpret ecm

        For Each Fields In DataECM.ResultData
            ECM = New clsECM
            ECM.ObjectDataLink.Connect(ECMs)
            ECM.TurretObjectDataLink.Connect(Turrets)
            ECM.Code = Fields(0)
            ECM.Name = Fields(1)
            InvariantParse_int(Fields(3), ECM.HitPoints)
            ECM.Designable = False
            ECM.Attachment.Models.Add(GetModelForPIE(PIE_List, Fields(5).ToLower, ReturnResult)) 'SensorModel
        Next

        'interpret feature

        For Each Fields In DataFeatures.ResultData
            FeatureType = New clsFeatureType
            FeatureType.UnitType_ObjectDataLink.Connect(UnitTypes)
            FeatureType.FeatureType_ObjectDataLink.Connect(FeatureTypes)
            FeatureType.Code = Fields(0)
            If Fields(9) = "OIL RESOURCE" Then 'type
                FeatureType.FeatureType = clsFeatureType.enumFeatureType.OilResource
            End If
            FeatureType.Name = Fields(1)
            If Not InvariantParse_int(Fields(10), FeatureType.Footprint.X) Then 'Width
                ReturnResult.WarningAdd("Feature footprint-x was not an integer for " & FeatureType.Code & ".")
            End If
            If Not InvariantParse_int(Fields(11), FeatureType.Footprint.Y) Then 'Breadth
                ReturnResult.WarningAdd("Feature footprint-y was not an integer for " & FeatureType.Code & ".")
            End If
            FeatureType.BaseAttachment = New clsUnitType.clsAttachment
            BaseAttachment = FeatureType.BaseAttachment
            Text = Fields(4).ToLower 'Model
            Attachment = BaseAttachment.CreateAttachment()
            Attachment.Models.Add(GetModelForPIE(PIE_List, Text, ReturnResult))
        Next

        'interpret structure

        For Each Fields In DataStructures.ResultData
            Dim StructureCode As String = Fields(0)
            Dim StructureTypeText As String = Fields(9) 'type
            Dim StructurePIEs() As String = Fields(22).ToLower.Split(","c)
            Dim StructureFootprint As sXY_int
            Dim StructureBasePIE As String = Fields(12).ToLower 'BaseModel
            If Not InvariantParse_int(Fields(10), StructureFootprint.X) Then 'Width
                ReturnResult.WarningAdd("Structure footprint-x was not an integer for " & StructureCode & ". " & Fields(10))
            End If
            If Not InvariantParse_int(Fields(11), StructureFootprint.Y) Then 'Breadth
                ReturnResult.WarningAdd("Structure footprint-y was not an integer for " & StructureCode & ". " & Fields(11))
            End If
            If StructureTypeText <> "WALL" Or StructurePIEs.GetLength(0) <> 4 Then
                'this is NOT a generic wall
                StructureType = New clsStructureType
                StructureType.UnitType_ObjectDataLink.Connect(UnitTypes)
                StructureType.StructureType_ObjectDataLink.Connect(StructureTypes)
                StructureType.Code = StructureCode
                StructureType.Name = Fields(1)
                StructureType.Footprint = StructureFootprint
                Select Case StructureTypeText
                    Case "DEMOLISH"
                        StructureType.StructureType = clsStructureType.enumStructureType.Demolish
                    Case "WALL"
                        StructureType.StructureType = clsStructureType.enumStructureType.Wall
                    Case "CORNER WALL"
                        StructureType.StructureType = clsStructureType.enumStructureType.CornerWall
                    Case "FACTORY"
                        StructureType.StructureType = clsStructureType.enumStructureType.Factory
                    Case "CYBORG FACTORY"
                        StructureType.StructureType = clsStructureType.enumStructureType.CyborgFactory
                    Case "VTOL FACTORY"
                        StructureType.StructureType = clsStructureType.enumStructureType.VTOLFactory
                    Case "COMMAND"
                        StructureType.StructureType = clsStructureType.enumStructureType.Command
                    Case "HQ"
                        StructureType.StructureType = clsStructureType.enumStructureType.HQ
                    Case "DEFENSE"
                        StructureType.StructureType = clsStructureType.enumStructureType.Defense
                    Case "POWER GENERATOR"
                        StructureType.StructureType = clsStructureType.enumStructureType.PowerGenerator
                    Case "POWER MODULE"
                        StructureType.StructureType = clsStructureType.enumStructureType.PowerModule
                    Case "RESEARCH"
                        StructureType.StructureType = clsStructureType.enumStructureType.Research
                    Case "RESEARCH MODULE"
                        StructureType.StructureType = clsStructureType.enumStructureType.ResearchModule
                    Case "FACTORY MODULE"
                        StructureType.StructureType = clsStructureType.enumStructureType.FactoryModule
                    Case "DOOR"
                        StructureType.StructureType = clsStructureType.enumStructureType.DOOR
                    Case "REPAIR FACILITY"
                        StructureType.StructureType = clsStructureType.enumStructureType.RepairFacility
                    Case "SAT UPLINK"
                        StructureType.StructureType = clsStructureType.enumStructureType.DOOR
                    Case "REARM PAD"
                        StructureType.StructureType = clsStructureType.enumStructureType.RearmPad
                    Case "MISSILE SILO"
                        StructureType.StructureType = clsStructureType.enumStructureType.MissileSilo
                    Case "RESOURCE EXTRACTOR"
                        StructureType.StructureType = clsStructureType.enumStructureType.ResourceExtractor
                    Case Else
                        StructureType.StructureType = clsStructureType.enumStructureType.Unknown
                End Select

                BaseAttachment = StructureType.BaseAttachment
                If StructurePIEs.GetLength(0) > 0 Then
                    BaseAttachment.Models.Add(GetModelForPIE(PIE_List, StructurePIEs(0), ReturnResult))
                End If
                StructureType.StructureBasePlate = GetModelForPIE(PIE_List, StructureBasePIE, ReturnResult)
                If BaseAttachment.Models.Count = 1 Then
                    If BaseAttachment.Models.Item(0).ConnectorCount >= 1 Then
                        Connector = BaseAttachment.Models.Item(0).Connectors(0)
                        Dim StructureWeapons() As String
                        StructureWeapons = Fields(21).Split(","c)
                        If StructureWeapons(0) <> Nothing Then
                            Weapon = FindWeaponCode(StructureWeapons(0))
                        Else
                            Weapon = Nothing
                        End If
                        ECM = FindECMCode(Fields(13)) 'ECMId
                        Sensor = FindSensorCode(Fields(14)) 'SensorId
                        If Weapon IsNot Nothing Then
                            If Weapon.Code <> "ZNULLWEAPON" Then
                                Attachment = BaseAttachment.CopyAttachment(Weapon.Attachment)
                                Attachment.Pos_Offset = Connector
                            End If
                        End If
                        If ECM IsNot Nothing Then
                            If ECM.Code <> "ZNULLECM" Then
                                Attachment = BaseAttachment.CopyAttachment(ECM.Attachment)
                                Attachment.Pos_Offset = Connector
                            End If
                        End If
                        If Sensor IsNot Nothing Then
                            If Sensor.Code <> "ZNULLSENSOR" Then
                                Attachment = BaseAttachment.CopyAttachment(Sensor.Attachment)
                                Attachment.Pos_Offset = Connector
                            End If
                        End If
                    End If
                End If
            Else
                'this is a generic wall
                Dim NewWall As New clsWallType
                NewWall.WallType_ObjectDataLink.Connect(WallTypes)
                NewWall.Code = StructureCode
                NewWall.Name = Fields(1)
                Dim WallBasePlate As clsModel = GetModelForPIE(PIE_List, StructureBasePIE, ReturnResult)

                Dim WallNum As Integer
                Dim WallStructureType As clsStructureType
                For WallNum = 0 To 3
                    WallStructureType = New clsStructureType
                    WallStructureType.UnitType_ObjectDataLink.Connect(UnitTypes)
                    WallStructureType.StructureType_ObjectDataLink.Connect(StructureTypes)
                    WallStructureType.WallLink.Connect(NewWall.Segments)
                    WallStructureType.Code = StructureCode
                    Text = NewWall.Name
                    Select Case WallNum
                        Case 0
                            Text &= " - "
                        Case 1
                            Text &= " + "
                        Case 2
                            Text &= " T "
                        Case 3
                            Text &= " L "
                    End Select
                    WallStructureType.Name = Text
                    WallStructureType.Footprint = StructureFootprint
                    WallStructureType.StructureType = clsStructureType.enumStructureType.Wall

                    BaseAttachment = WallStructureType.BaseAttachment

                    Text = StructurePIEs(WallNum)
                    BaseAttachment.Models.Add(GetModelForPIE(PIE_List, Text, ReturnResult))
                    WallStructureType.StructureBasePlate = WallBasePlate
                Next
            End If
        Next

        'interpret templates

        Dim TurretConflictCount As Integer = 0
        For Each Fields In DataTemplates.ResultData
            Template = New clsDroidTemplate
            Template.UnitType_ObjectDataLink.Connect(UnitTypes)
            Template.DroidTemplate_ObjectDataLink.Connect(DroidTemplates)
            Template.Code = Fields(0)
            Template.Name = Fields(1)
            Select Case Fields(9) 'type
                Case "ZNULLDROID"
                    Template.TemplateDroidType = TemplateDroidType_Null
                Case "DROID"
                    Template.TemplateDroidType = TemplateDroidType_Droid
                Case "CYBORG"
                    Template.TemplateDroidType = TemplateDroidType_Cyborg
                Case "CYBORG_CONSTRUCT"
                    Template.TemplateDroidType = TemplateDroidType_CyborgConstruct
                Case "CYBORG_REPAIR"
                    Template.TemplateDroidType = TemplateDroidType_CyborgRepair
                Case "CYBORG_SUPER"
                    Template.TemplateDroidType = TemplateDroidType_CyborgSuper
                Case "TRANSPORTER"
                    Template.TemplateDroidType = TemplateDroidType_Transporter
                Case "PERSON"
                    Template.TemplateDroidType = TemplateDroidType_Person
                Case "DROID_COMMAND"
                    Template.TemplateDroidType = TemplateDroidType_Droid
                Case "CONSTRUCT"
                    Template.TemplateDroidType = TemplateDroidType_Droid
                Case Else
                    Template.TemplateDroidType = TemplateDroidType_Droid
                    ReturnResult.WarningAdd("Template " & Template.GetDisplayTextCode & " had an unrecognised type: " & Fields(9) & ". Defaulting to Droid")
            End Select
            Dim LoadPartsArgs As New clsDroidDesign.sLoadPartsArgs
            LoadPartsArgs.Body = FindBodyCode(Fields(15)) 'Body
            LoadPartsArgs.Brain = FindBrainCode(Fields(16)) 'Brain
            LoadPartsArgs.Construct = FindConstructorCode(Fields(17)) 'Construct
            LoadPartsArgs.ECM = FindECMCode(Fields(13)) 'ECMId
            LoadPartsArgs.Propulsion = FindPropulsionCode(Fields(20)) 'Propulsion
            LoadPartsArgs.Repair = FindRepairCode(Fields(18)) 'Repair
            LoadPartsArgs.Sensor = FindSensorCode(Fields(19)) 'Sensor
            Dim TemplateWeapons() As String = Fields(21).Split(","c)
            Try
                If TemplateWeapons(0) <> Nothing Then
                    LoadPartsArgs.Weapon1 = FindWeaponCode(TemplateWeapons(0))
                End If
                If TemplateWeapons(1) <> Nothing Then
                    LoadPartsArgs.Weapon2 = FindWeaponCode(TemplateWeapons(1))
                End If
                If TemplateWeapons(2) <> Nothing Then
                    LoadPartsArgs.Weapon3 = FindWeaponCode(TemplateWeapons(2))
                End If
            Catch ex As Exception
                'do nothing because we know not everything will have all 3 weapons :)
            End Try
            If Not Template.LoadParts(LoadPartsArgs) Then
                If TurretConflictCount < 16 Then
                    ReturnResult.WarningAdd("Template " & Template.GetDisplayTextCode & " had multiple conflicting turrets.")
                End If
                TurretConflictCount += 1
            End If
        Next
        If TurretConflictCount > 0 Then
            ReturnResult.WarningAdd(TurretConflictCount & " templates had multiple conflicting turrets.")
        End If

        Return ReturnResult
    End Function

    Public Function GetRowsWithValue(TextLines As SimpleList(Of String()), Value As String) As SimpleList(Of String())
        Dim Result As New SimpleList(Of String())

        Dim Line() As String
        For Each Line In TextLines
            If Line(0) = Value Then
                Result.Add(Line)
            End If
        Next

        Return Result
    End Function

    Public Structure sBytes
        Public Bytes() As Byte
    End Structure
    Public Structure sLines
        Public Lines() As String

        Public Sub RemoveComments()
            Dim LineNum As Integer
            Dim LineCount As Integer = Lines.GetUpperBound(0) + 1
            Dim InCommentBlock As Boolean
            Dim CommentStart As Integer
            Dim CharNum As Integer
            Dim CommentLength As Integer

            For LineNum = 0 To LineCount - 1
                CharNum = 0
                If InCommentBlock Then
                    CommentStart = 0
                End If
                Do
                    If CharNum >= Lines(LineNum).Length Then
                        If InCommentBlock Then
                            Lines(LineNum) = Strings.Left(Lines(LineNum), CommentStart)
                        End If
                        Exit Do
                    ElseIf InCommentBlock Then
                        If Lines(LineNum).Chars(CharNum) = "*"c Then
                            CharNum += 1
                            If CharNum >= Lines(LineNum).Length Then

                            ElseIf Lines(LineNum).Chars(CharNum) = "/"c Then
                                CharNum += 1
                                CommentLength = CharNum - CommentStart
                                InCommentBlock = False
                                Lines(LineNum) = Strings.Left(Lines(LineNum), CommentStart) & Strings.Right(Lines(LineNum), Lines(LineNum).Length - (CommentStart + CommentLength))
                                CharNum -= CommentLength
                            End If
                        Else
                            CharNum += 1
                        End If
                    ElseIf Lines(LineNum).Chars(CharNum) = "/"c Then
                        CharNum += 1
                        If CharNum >= Lines(LineNum).Length Then

                        ElseIf Lines(LineNum).Chars(CharNum) = "/"c Then
                            CommentStart = CharNum - 1
                            CharNum = Lines(LineNum).Length
                            CommentLength = CharNum - CommentStart
                            Lines(LineNum) = Strings.Left(Lines(LineNum), CommentStart) & Strings.Right(Lines(LineNum), Lines(LineNum).Length - (CommentStart + CommentLength))
                            Exit Do
                        ElseIf Lines(LineNum).Chars(CharNum) = "*"c Then
                            CommentStart = CharNum - 1
                            CharNum += 1
                            InCommentBlock = True
                        End If
                    Else
                        CharNum += 1
                    End If
                Loop
            Next
        End Sub
    End Structure

    Public Function GetModelForPIE(PIE_List As SimpleList(Of clsPIE), PIE_LCaseFileTitle As String, ResultOutput As clsResult) As clsModel

        If PIE_LCaseFileTitle = "0" Or PIE_LCaseFileTitle = "znullbody.pie" Or PIE_LCaseFileTitle = "znullgun.pie" Or PIE_LCaseFileTitle = "znullturret.pie" Then
            Return Nothing
        End If

        Dim A As Integer
        Dim PIEFile As IO.StreamReader
        Dim PIE As clsPIE

        Dim Result As New clsResult("Loading PIE file " & PIE_LCaseFileTitle)

        For A = 0 To PIE_List.Count - 1
            PIE = PIE_List(A)
            If PIE.LCaseFileTitle = PIE_LCaseFileTitle Then
                If PIE.Model Is Nothing Then
                    PIE.Model = New clsModel
                    Try
                        PIEFile = New IO.StreamReader(PIE.Path)
                        Try
                            Result.Take(PIE.Model.ReadPIE(PIEFile, Me))
                        Catch ex As Exception
                            PIEFile.Close()
                            Result.WarningAdd(ex.Message)
                            ResultOutput.Add(Result)
                            Return PIE.Model
                        End Try
                    Catch ex As Exception
                        Result.WarningAdd(ex.Message)
                    End Try
                End If
                ResultOutput.Add(Result)
                Return PIE.Model
            End If
        Next

        If Not Result.HasWarnings Then
            Result.WarningAdd("file is missing")
        End If
        ResultOutput.Add(Result)

        Return Nothing
    End Function

    Public Sub SetComponentName(Names As SimpleList(Of String()), Component As clsComponent, Result As clsResult)
        Dim ValueSearchResults As SimpleList(Of String())

        ValueSearchResults = GetRowsWithValue(Names, Component.Code)
        If ValueSearchResults.Count = 0 Then
            Result.WarningAdd("No name for component " & Component.Code & ".")
        Else
            Component.Name = ValueSearchResults(0)(1)
        End If
    End Sub

    Public Sub SetFeatureName(Names As SimpleList(Of String()), FeatureType As clsFeatureType, Result As clsResult)
        Dim ValueSearchResults As SimpleList(Of String())

        ValueSearchResults = GetRowsWithValue(Names, FeatureType.Code)
        If ValueSearchResults.Count = 0 Then
            Result.WarningAdd("No name for feature type " & FeatureType.Code & ".")
        Else
            FeatureType.Name = ValueSearchResults(0)(1)
        End If
    End Sub

    Public Sub SetStructureName(Names As SimpleList(Of String()), StructureType As clsStructureType, Result As clsResult)
        Dim ValueSearchResults As SimpleList(Of String())

        ValueSearchResults = GetRowsWithValue(Names, StructureType.Code)
        If ValueSearchResults.Count = 0 Then
            Result.WarningAdd("No name for structure type " & StructureType.Code & ".")
        Else
            StructureType.Name = ValueSearchResults(0)(1)
        End If
    End Sub

    Public Sub SetTemplateName(Names As SimpleList(Of String()), Template As clsDroidTemplate, Result As clsResult)
        Dim ValueSearchResults As SimpleList(Of String())

        ValueSearchResults = GetRowsWithValue(Names, Template.Code)
        If ValueSearchResults.Count = 0 Then
            Result.WarningAdd("No name for droid template " & Template.Code & ".")
        Else
            Template.Name = ValueSearchResults(0)(1)
        End If
    End Sub

    Public Sub SetWallName(Names As SimpleList(Of String()), WallType As clsWallType, Result As clsResult)
        Dim ValueSearchResults As SimpleList(Of String())

        ValueSearchResults = GetRowsWithValue(Names, WallType.Code)
        If ValueSearchResults.Count = 0 Then
            Result.WarningAdd("No name for structure type " & WallType.Code & ".")
        Else
            WallType.Name = ValueSearchResults(0)(1)
        End If
    End Sub

    Public Function FindBodyCode(Code As String) As clsBody
        Dim Component As clsBody

        For Each Component In Bodies
            If Component.Code = Code Then
                Return Component
            End If
        Next

        Return Nothing
    End Function

    Public Function FindPropulsionCode(Code As String) As clsPropulsion
        Dim Component As clsPropulsion

        For Each Component In Propulsions
            If Component.Code = Code Then
                Return Component
            End If
        Next

        Return Nothing
    End Function

    Public Function FindConstructorCode(Code As String) As clsConstruct
        Dim Component As clsConstruct

        For Each Component In Constructors
            If Component.Code = Code Then
                Return Component
            End If
        Next

        Return Nothing
    End Function

    Public Function FindSensorCode(Code As String) As clsSensor
        Dim Component As clsSensor

        For Each Component In Sensors
            If Component.Code = Code Then
                Return Component
            End If
        Next

        Return Nothing
    End Function

    Public Function FindRepairCode(Code As String) As clsRepair
        Dim Component As clsRepair

        For Each Component In Repairs
            If Component.Code = Code Then
                Return Component
            End If
        Next

        Return Nothing
    End Function

    Public Function FindECMCode(Code As String) As clsECM
        Dim Component As clsECM

        For Each Component In ECMs
            If Component.Code = Code Then
                Return Component
            End If
        Next

        Return Nothing
    End Function

    Public Function FindBrainCode(Code As String) As clsBrain
        Dim Component As clsBrain

        For Each Component In Brains
            If Component.Code = Code Then
                Return Component
            End If
        Next

        Return Nothing
    End Function

    Public Function FindWeaponCode(Code As String) As clsWeapon
        Dim Component As clsWeapon

        For Each Component In Weapons
            If Component.Code = Code Then
                Return Component
            End If
        Next

        Return Nothing
    End Function

    Public Function Get_TexturePage_GLTexture(FileTitle As String) As Integer
        Dim LCaseTitle As String = FileTitle.ToLower
        Dim TexPage As clsTexturePage

        For Each TexPage In TexturePages
            If TexPage.FileTitle.ToLower = LCaseTitle Then
                Return TexPage.GLTexture_Num
            End If
        Next
        Return 0
    End Function

    Public Function FindOrCreateWeapon(Code As String) As clsWeapon
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

    Public Function FindOrCreateConstruct(Code As String) As clsConstruct
        Dim Result As clsConstruct

        Result = FindConstructorCode(Code)
        If Result IsNot Nothing Then
            Return Result
        End If
        Result = New clsConstruct
        Result.IsUnknown = True
        Result.Code = Code
        Return Result
    End Function

    Public Function FindOrCreateRepair(Code As String) As clsRepair
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

    Public Function FindOrCreateSensor(Code As String) As clsSensor
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

    Public Function FindOrCreateBrain(Code As String) As clsBrain
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

    Public Function FindOrCreateECM(Code As String) As clsECM
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

    Public Function FindOrCreateTurret(TurretType As clsTurret.enumTurretType, TurretCode As String) As clsTurret

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

    Public Function FindOrCreateBody(Code As String) As clsBody
        Dim Result As clsBody

        Result = FindBodyCode(Code)
        If Result IsNot Nothing Then
            Return Result
        End If
        Result = New clsBody
        Result.IsUnknown = True
        Result.Code = Code
        Return Result
    End Function

    Public Function FindOrCreatePropulsion(Code As String) As clsPropulsion
        Dim Result As clsPropulsion

        Result = FindPropulsionCode(Code)
        If Result IsNot Nothing Then
            Return Result
        End If
        Result = New clsPropulsion(Bodies.Count)
        Result.IsUnknown = True
        Result.Code = Code
        Return Result
    End Function

    Public Function FindOrCreateUnitType(Code As String, Type As clsUnitType.enumType, WallType As Integer) As clsUnitType

        Select Case Type
            Case clsUnitType.enumType.Feature
                Dim FeatureType As clsFeatureType
                For Each FeatureType In FeatureTypes
                    If FeatureType.Code = Code Then
                        Return FeatureType
                    End If
                Next
                FeatureType = New clsFeatureType
                FeatureType.IsUnknown = True
                FeatureType.Code = Code
                FeatureType.Footprint.X = 1
                FeatureType.Footprint.Y = 1
                Return FeatureType
            Case clsUnitType.enumType.PlayerStructure
                Dim StructureType As clsStructureType
                For Each StructureType In StructureTypes
                    If StructureType.Code = Code Then
                        If WallType < 0 Then
                            Return StructureType
                        ElseIf StructureType.WallLink.IsConnected Then
                            If StructureType.WallLink.ArrayPosition = WallType Then
                                Return StructureType
                            End If
                        End If
                    End If
                Next
                StructureType = New clsStructureType
                StructureType.IsUnknown = True
                StructureType.Code = Code
                StructureType.Footprint.X = 1
                StructureType.Footprint.Y = 1
                Return StructureType
            Case clsUnitType.enumType.PlayerDroid
                Dim DroidType As clsDroidTemplate
                For Each DroidType In DroidTemplates
                    If DroidType.IsTemplate Then
                        If DroidType.Code = Code Then
                            Return DroidType
                        End If
                    End If
                Next
                DroidType = New clsDroidTemplate
                DroidType.IsUnknown = True
                DroidType.Code = Code
                Return DroidType
            Case Else
                Return Nothing
        End Select
    End Function

    Public Function FindFirstStructureType(Type As clsStructureType.enumStructureType) As clsStructureType
        Dim StructureType As clsStructureType

        For Each StructureType In StructureTypes
            If StructureType.StructureType = Type Then
                Return StructureType
            End If
        Next

        Return Nothing
    End Function
End Class
