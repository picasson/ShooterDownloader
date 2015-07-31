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

#pragma once
#include "resource.h"       // main symbols

#include "ShooterExt_i.h"

#include <string>
#include <list>
typedef std::list< std::basic_string<TCHAR> > StringList;


// CShooterContextMenuExt

class ATL_NO_VTABLE CShooterContextMenuExt :
	public CComObjectRootEx<CComSingleThreadModel>,
	public CComCoClass<CShooterContextMenuExt, &CLSID_ShooterContextMenuExt>,
	public IShellExtInit,
	public IContextMenu
{
public:
CShooterContextMenuExt();

DECLARE_REGISTRY_RESOURCEID(IDR_SHOOTERCONTEXTMENUEXT)

DECLARE_NOT_AGGREGATABLE(CShooterContextMenuExt)

BEGIN_COM_MAP(CShooterContextMenuExt)
	COM_INTERFACE_ENTRY(IShellExtInit)
	COM_INTERFACE_ENTRY(IContextMenu)
END_COM_MAP()



	DECLARE_PROTECT_FINAL_CONSTRUCT()

	HRESULT FinalConstruct()
	{
		return S_OK;
	}

	void FinalRelease()
	{
	}

public:
	//IShellExtInit
	STDMETHODIMP Initialize(LPCITEMIDLIST, LPDATAOBJECT, HKEY);

	// IContextMenu
	STDMETHODIMP GetCommandString(UINT_PTR, UINT, UINT*, LPSTR, UINT);
	STDMETHODIMP InvokeCommand(LPCMINVOKECOMMANDINFO);
	STDMETHODIMP QueryContextMenu(HMENU, UINT, UINT, UINT, UINT);

private:
	bool IsDir(TCHAR* path);
	StringList m_fileList;
	HBITMAP    m_hIcon;
	bool m_bHasDir;

};

const static TCHAR SHOOTER_DLDR_FILE_NAME[] = _T("ShooterDownloader.exe");

OBJECT_ENTRY_AUTO(__uuidof(ShooterContextMenuExt), CShooterContextMenuExt)
