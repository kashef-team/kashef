create table WcfEvents
(
	MessageID uniqueidentifier primary key not null,
	ProcessID int not null,
	ThreadID int not null,
	TimeCreated datetime not null,
	ServiceName nvarchar(512) not null,
	ServiceMachineName nvarchar(256) not null,
	ServiceUri nvarchar(512) not null,
	ServiceIP nvarchar(512) not null,
	ServicePort int not null,
	ServiceIdentity nvarchar(256) not null,
	ServiceAuthenticationType nvarchar(256) not null,
	ClientIP nvarchar(256) not null,
	ClientPort int not null,
	ClientIdentity nvarchar(256) not null,
	ClientAuthenticationType nvarchar(256) not null,
	[Action] nvarchar(512) not null,
	Request xml,
	Response xml,
	Misc xml,
	IsFault bit
)

go

Create procedure AddWcfEvent 
(
	@MessageID uniqueidentifier,
	@ProcessID  int,
	@ThreadID int,
    @TimeCreated datetime,
    @ServiceName nvarchar(512),
    @ServiceMachineName nvarchar(256),
    @ServiceUri nvarchar(512),
    @ServiceIP nvarchar(512),
    @ServicePort int,
    @ServiceIdentity nvarchar(256),
    @ServiceAuthenticationType nvarchar(256),
    @ClientIP nvarchar(256),
    @ClientPort int,
    @ClientIdentity nvarchar(256),
    @ClientAuthenticationType nvarchar(256),
    @Action nvarchar(512),
    @Request xml,
    @Response xml,
    @Misc xml,
    @IsFault bit
)
as
INSERT INTO [dbo].[WcfEvents]
           ([MessageID]
           ,[ProcessID]
           ,[ThreadID]
           ,[TimeCreated]
           ,[ServiceName]
           ,[ServiceMachineName]
           ,[ServiceUri]
           ,[ServiceIP]
           ,[ServicePort]
           ,[ServiceIdentity]
           ,[ServiceAuthenticationType]
           ,[ClientIP]
           ,[ClientPort]
           ,[ClientIdentity]
           ,[ClientAuthenticationType]
           ,[Action]
           ,[Request]
           ,[Response]
           ,[Misc]
           ,[IsFault])
     VALUES
           (
		   @MessageID, 
           @ProcessID,
           @ThreadID,
           @TimeCreated,
           @ServiceName,
           @ServiceMachineName,
           @ServiceUri,
           @ServiceIP,
           @ServicePort,
           @ServiceIdentity,
           @ServiceAuthenticationType,
           @ClientIP,
           @ClientPort,
           @ClientIdentity,
           @ClientAuthenticationType,
           @Action, 
           @Request,
           @Response,
           @Misc,
           @IsFault
		   )