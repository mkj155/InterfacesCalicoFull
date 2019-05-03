CREATE TABLE [dbo].[BIANCHI_PROCESS](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[inicio] [datetime] NULL,
	[fin] [datetime] NULL,
	[cant_lineas] [numeric](18, 0) NULL,
	[estado] [varchar](50) NULL,
	[process_id] [numeric](18, 0) NULL,
	[maquina] [varchar](50) NULL,
	[interfaz] [varchar](50) NULL,
	[fecha_ultima] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]