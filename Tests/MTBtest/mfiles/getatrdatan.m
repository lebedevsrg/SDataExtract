function ftsdata=getatrdatan(sec,tf,from,to,varargin)
asm1 = NET.addAssembly('System.Core');
asm2 = NET.addAssembly('System.Globalization');
asm3 = NET.addAssembly('D:\SH\SDataExtract\bin\x64\Release\SDataExt.dll');

%% New func to get range of trading data from S#.Data storage
% ftsdata=getatrdatan(sec,tf,from,to,mode,dtype,dpath)
% - Obligatory input args
% sec - trading paper
% tf - time frame: 
%       'm1' - 1 minute
%       'm15' - 15 minute
%       'h' - 60 minute
%       'd' - load daily data 
% from - date From
% to - date TO
% - Optional input args
% mode - format of results
%       'fts' - fin time series
%       'arr' - array format
%       'tbl' - table format
% dtype - type of data: 
%       'ALL','A'   - full HLOCV values 
%       'CLOSE','C' - only Close values
%       'OPEN','O'  - only Open values
%       'VOL','V'  - only Volume values
% dpath - path to S#.Data storage
% - Examples
% nlTSH1 =  getatrdatan('SPFB.BR@FORTS','h1','04.07.2014','02.02.2015'); 
% nlTSH1 =  getatrdatan('SPFB.RTS@FORTS','m1','04.07.2014','02.02.2015','tbl')

%% Check input args
n=nargin;
switch n
    case 4
        mode ='fts';
        DType = 'A';
        DPath = 'D:\\SH\\Hydra';
    case 5
        mode = varargin{1};
        DType = 'A';
        DPath = 'D:\\SH\\Hydra';
    case 6
        mode = varargin{1};
        DType = varargin{2};
        DPath = 'D:\\SH\\Hydra';
    case 7
        mode = varargin{1};
        DType = varargin{2};
        DPath = varargin{3};
    case 8
        disp('To many optional args'); return; 
end

%% Get Data
dl= SDataExt.DataLoader;
dl.Code=sec;
dl.DType=DType;
dl.Path=DPath;

switch tf
    case 'm1'  
        dl.TimeFrame=System.TimeSpan.FromMinutes(1);
    case 'm15' 
        dl.TimeFrame=System.TimeSpan.FromMinutes(15);
    case 'h1'  
        dl.TimeFrame=System.TimeSpan.FromHours(1);
    case 'd'   
        dl.TimeFrame=System.TimeSpan.FromDays(1);
    otherwise
        dl.TimeFrame=System.TimeSpan.FromDays(1);
end

dtf = System.Globalization.DateTimeFormatInfo.CurrentInfo;
dl.From = System.DateTime.Parse(from,dtf);
dl.To = System.DateTime.Parse(to,dtf);
res=dl.GetData;

if res==1
    fdata=double(dl.dlInfo)';
    fdates=double(dl.dlDates)';
else
    disp ('Inconsistency of data')
    ftsdata = 0;
    return;
end
        
%% Trim data to output format

switch DType
    case {'A','ALL'}
        FNames={'OPEN' 'HIGH' 'LOW' 'CLOSE' 'VOL'};
    case {'C','CLOSE'}
        FNames={'CLOSE'};
    case {'O','OPEN'}
        FNames={'OPEN'};
    case {'V','VOL'}
        FNames={'VOL'};
end

switch mode
    case 'fts'
          ftsdata = fints(fdates,fdata,FNames, 0,fdata(1,2));
          ftsdata.freq=0;            
    case 'arr'
          ftsdata=fdata;
    case 'tbl'
          ftsdata=table(fdates,fdata(:,1),fdata(:,2),fdata(:,3),fdata(:,4),fdata(:,5),'VariableNames',['DATE' FNames]);
end

end