function seclist = getseclist(varargin)
asm1 = NET.addAssembly('System.Core');
asm2 = NET.addAssembly('System.Globalization');
asm3 = NET.addAssembly('D:\SH\SDataExtract\bin\x64\Release\SDataExt.dll');

%% Func to get cached secutities from S#.Data storage
% seclist = getseclist(dpath)
% - Optional input args
% dpath - path to S#.Data storage
% - Examples
% seclist = getseclist;

%%
n=nargin;
switch n
    case 0
        DPath = 'D:\\SH\\Hydra';
    case 4
        DPath = varargin{1};
    case 2
        disp('To many optional args'); return; 
end

%% Get Dats
dl= SDataExt.DataLoader;
dl.Path=DPath;
res=dl.GetList;

if res==1
    seclistdt=cell(dl.SecArray)';
    seclistcl=regexp(seclistdt,'\w*(?=@)','match');
    seclist = [cellfun(@char, seclistcl,'UniformOutput',0) seclistdt]
else
    disp ('Inconsistency of data')
    seclist = '';
end

    return;
end

