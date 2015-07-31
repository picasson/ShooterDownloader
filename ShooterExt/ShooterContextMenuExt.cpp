/*
 *   Shooter Subtitle Downloader: Automatic Subtitle Downloader for the http://shooter.cn.
 *   Copyright (C) 2009  John Fung
 *
 *   This program is free software: you can redistribute it and/or modify
 *   it under the terms of the GNU Affero General Public License as published by
 *   the Free Software Foundation, either version 3 of the License, or
 *   (at your option) any later version.
 *
 *   This program is distributed in the hope that it will be useful,
 *   but WITHOUT ANY WARRANTY; without even the implied warranty of
 *   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *   GNU Affero General Public License for more details.
 *
 *   You should have received a copy of the GNU Affero General Public License
 *   along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */

#include "stdafx.h"
#include "ShooterContextMenuExt.h"


// CShooterContextMenuExt

CShooterContextMenuExt::CShooterContextMenuExt()
{
	m_hIcon = LoadBitmap ( _AtlBaseModule.GetModuleInstance(),
		MAKEINTRESOURCE(IDB_SHOOTER) );
	m_bHasDir = false;
}

STDMETHODIMP CShooterContextMenuExt::Initialize ( 
  LPCITEMIDLIST pidlFolder,
  LPDATAOBJECT pDataObj,
  HKEY hProgID )
{
	TCHAR     szFile[MAX_PATH];
	FORMATETC fmt = { CF_HDROP, NULL, DVASPECT_CONTENT, -1, TYMED_HGLOBAL };
	STGMEDIUM stg = { TYMED_HGLOBAL };
	HDROP     hDrop;

	// Look for CF_HDROP data in the data object.
	if ( FAILED( pDataObj->GetData ( &fmt, &stg ) ))
	{
		// Nope! Return an "invalid argument" error back to Explorer.
		return E_INVALIDARG;
	}

	// Get a pointer to the actual data.
	hDrop = (HDROP) GlobalLock ( stg.hGlobal );

	// Make sure it worked.
	if ( NULL == hDrop )
	{
		ReleaseStgMedium ( &stg );
		return E_INVALIDARG;
	}

	// Sanity check - make sure there is at least one filename.
	UINT uNumFiles = DragQueryFile ( hDrop, 0xFFFFFFFF, NULL, 0 );
	HRESULT hr = S_OK;

	if ( 0 == uNumFiles )
	{
		GlobalUnlock ( stg.hGlobal );
		ReleaseStgMedium ( &stg );
		return E_INVALIDARG;
	}

	// Get the name of the first file and store it in our member variable m_szFile.
	m_bHasDir = false;

	for(UINT uFile = 0 ; uFile < uNumFiles ; uFile++)
	{
		if(0 == DragQueryFile(hDrop, uFile, szFile, MAX_PATH))
			continue;

		ATLTRACE("Checking file <%s>\n", szFile);

		m_fileList.push_back(szFile);

		if(IsDir(szFile))
			m_bHasDir = true;
	}


	GlobalUnlock ( stg.hGlobal );
	ReleaseStgMedium ( &stg );

	return hr;
}

STDMETHODIMP CShooterContextMenuExt::QueryContextMenu (
    HMENU hmenu, UINT uMenuIndex, UINT uidFirstCmd,
    UINT uidLastCmd, UINT uFlags )
{
	// If the flags include CMF_DEFAULTONLY then we shouldn't do anything.
    if ( uFlags & CMF_DEFAULTONLY )
        return MAKE_HRESULT ( SEVERITY_SUCCESS, FACILITY_NULL, 0 );

	InsertMenu ( hmenu, uMenuIndex, MF_SEPARATOR|MF_BYPOSITION, 0, NULL );

	UINT uidCmd = uidFirstCmd;
	uMenuIndex++;
    InsertMenu ( hmenu, uMenuIndex, MF_BYPOSITION, uidCmd, _T("下載字幕") );
	// Set the bitmap.
    if ( NULL != m_hIcon )
        SetMenuItemBitmaps ( hmenu, uMenuIndex, MF_BYPOSITION, m_hIcon, NULL );

	if(!m_bHasDir)
	{
		uMenuIndex++;
		uidCmd++;
		InsertMenu ( hmenu, uMenuIndex, MF_BYPOSITION, uidCmd, _T("字幕簡轉繁") );
		// Set the bitmap.
		if ( NULL != m_hIcon )
			SetMenuItemBitmaps ( hmenu, uMenuIndex, MF_BYPOSITION, m_hIcon, NULL );
	}

	uMenuIndex++;
	InsertMenu ( hmenu, uMenuIndex, MF_SEPARATOR|MF_BYPOSITION, 0, NULL );


    return MAKE_HRESULT ( SEVERITY_SUCCESS, FACILITY_NULL, uidCmd - uidFirstCmd + 1 );
}

STDMETHODIMP CShooterContextMenuExt::GetCommandString (
    UINT_PTR idCmd, UINT uFlags, UINT* pwReserved, LPSTR pszName, UINT cchMax )
{
	USES_CONVERSION;

	// Check idCmd, it must be 0 since we have only one menu item.
	if ( 0 != idCmd )
		return E_INVALIDARG;

	// If Explorer is asking for a help string, copy our string into the
	// supplied buffer.
	if ( uFlags & GCS_HELPTEXT )
	{
		LPCTSTR szText = _T("開啟射手網字幕下載工具");

		if ( uFlags & GCS_UNICODE )
		{
			// We need to cast pszName to a Unicode string, and then use the
			// Unicode string copy API.
			lstrcpynW ( (LPWSTR) pszName, T2CW(szText), cchMax );
		}
		else
		{
			// Use the ANSI string copy API to return the help string.
			lstrcpynA ( pszName, T2CA(szText), cchMax );
		}

		return S_OK;
	}

	return E_INVALIDARG;
}

STDMETHODIMP CShooterContextMenuExt::InvokeCommand ( LPCMINVOKECOMMANDINFO pCmdInfo )
{
	// If lpVerb really points to a string, ignore this function call and bail out.
	if ( 0 != HIWORD( pCmdInfo->lpVerb ) )
		return E_INVALIDARG;

	// Get the command index - the only valid one is 0.
	switch ( LOWORD( pCmdInfo->lpVerb) )
	{
	case 0:
	case 1:
		{
			TCHAR szShooterDir[MAX_PATH];
			TCHAR szShooterDldrPath[MAX_PATH];

			//Build the ShooterDownloader's file path from this module's path.
			//Limitation: ShooterDownloader must locate in the same dir as this module.
			HINSTANCE hModule = _AtlBaseModule.GetModuleInstance();
			GetModuleFileName((HMODULE) hModule, szShooterDir, sizeof(szShooterDir));
			TCHAR* pLastSlash = _tcsrchr(szShooterDir, _T('\\'));
			*(pLastSlash + 1) = _T('\0');
			_tcscpy_s(szShooterDldrPath, szShooterDir);
			_tcscat_s(szShooterDldrPath, SHOOTER_DLDR_FILE_NAME);

			TCHAR szTempDir[MAX_PATH], szTempFilePath[MAX_PATH];
			// Get the temp path.
			DWORD dwRetVal = GetTempPath(MAX_PATH,     // length of the buffer
				szTempDir); // buffer for path 
			if (dwRetVal > MAX_PATH || (dwRetVal == 0))
			{
				return E_FAIL;
			}

			// Create a temporary file. 
			UINT uRetVal = GetTempFileName(szTempDir, // directory for tmp files
				TEXT("SDL"),  // temp file name prefix 
				0,            // create unique name 
				szTempFilePath);  // buffer for name 
			if (uRetVal == 0)
			{
				return E_FAIL;
			}

			//Write file list to a temp file.
			FILE* fp; 
			errno_t ret = _tfopen_s(&fp, szTempFilePath, _T("w, ccs=UTF-8"));
			if(ret != 0)
			{
				return E_FAIL;
			}
			
			StringList::const_iterator itor;
			for(itor = m_fileList.begin(); itor != m_fileList.end(); itor++)
			{
				_ftprintf_s(fp, _T("%s\n"), itor->c_str());
			}
			fclose(fp);

			//Call ShooterDownloader and pass it the file list.
			const static int PARAM_SIZE = 512;
			TCHAR param[PARAM_SIZE];
			if(LOWORD( pCmdInfo->lpVerb) == 0)
			{
				//download subtitle
				_stprintf_s(param, PARAM_SIZE, _T("-lst=\"%s\" /r"), szTempFilePath);
			}
			else
			{
				//convert subtitle
				_stprintf_s(param, PARAM_SIZE, _T("-lst=\"%s\" /r /c"), szTempFilePath);
			}

			ShellExecute(NULL, _T("Open"), szShooterDldrPath, param, NULL, SW_SHOWNORMAL);

			return S_OK;
		}
		break;

	default:
		return E_INVALIDARG;
		break;
	}
}

bool CShooterContextMenuExt::IsDir(TCHAR* path)
{
	WIN32_FIND_DATA findData;
	HANDLE hFile = FindFirstFile(path, &findData);

	return (hFile != INVALID_HANDLE_VALUE) && (findData.dwFileAttributes & FILE_ATTRIBUTE_DIRECTORY);
}
