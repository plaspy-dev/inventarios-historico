"""Prepare a disposable, synthetic-only IIS Express site outside the checkout."""
import argparse
import os
from pathlib import Path
import re
from shutil import copy2, copytree, ignore_patterns
import subprocess
import xml.etree.ElementTree as ET

root = Path(__file__).resolve().parents[1]
parser = argparse.ArgumentParser(description=__doc__)
parser.add_argument('output', type=Path)
destination = parser.parse_args().output.resolve()
if destination.is_relative_to(root) or destination.exists():
    parser.error('Choose a new directory outside the checkout.')
secret = os.environ.get('INVENTARIO_DEMO_LOGIN', '')
if not re.fullmatch(r'[a-f0-9]{16,128}',secret):
    parser.error('Generate the local hexadecimal test password INVENTARIO_DEMO_LOGIN.')
site = destination / 'inventarios'
copytree(root / 'inventarios',site,ignore=ignore_patterns('Web.config','App_Data','obj'))
library = destination / 'src/SDG.Componentes'
copytree(root / 'src/SDG.Componentes',library,ignore=ignore_patterns('bin','obj'))
subprocess.run(['dotnet','build',str(library/'SDG.Componentes.csproj'),'-c','Release','--nologo'],check=True)
copy2(library/'bin/Release/net48/SDG.Componentes.dll',site/'Bin')
cfg=ET.parse(root/'demo/Web.config.example')
section=cfg.getroot().find('./system.web/compilation')
if section is not None:
    section.set('debug','true')
connections=ET.Element('connectionStrings')
for name in ('MySqlConnectionString','ConexionUsuario'):
    ET.SubElement(connections,'add',name=name,
        connectionString='Server=127.0.0.1;Port=53306;Database=demo_inventory;'
                         f'Uid=inventario_demo;Pwd={secret};Pooling=false;',
        providerName='MySql.Data.MySqlClient')
cfg.getroot().insert(0,connections)
providers=ET.SubElement(ET.SubElement(cfg.getroot(),'system.data'),'DbProviderFactories')
ET.SubElement(providers,'remove',invariant='MySql.Data.MySqlClient')
ET.SubElement(providers,'add',name='MySQL Connector/NET',invariant='MySql.Data.MySqlClient',
              description='Local demo provider',type='MySql.Data.MySqlClient.MySqlClientFactory, MySql.Data')
settings=ET.SubElement(cfg.getroot(),'appSettings')
formats=destination/'formatos-demo'
formats.mkdir()
for key,value in (('InventoryDatabase','demo_inventory'),('Usuario','CUsuario'),
                  ('ruta_formatos',str(formats)),('ValidationSettings:UnobtrusiveValidationMode','None')):
    ET.SubElement(settings,'add',key=key,value=value)
cfg.write(site/'Web.config',encoding='utf-8',xml_declaration=True)
user=site/'App_Code/CUsuario.cs'
source=user.read_bytes()
source=re.sub(rb'(\b(?:db|szDataBase)\s*=\s*)"([A-Za-z]+)"(?=;)',
              lambda match:match[1]+b'"demo_inventory"',source,flags=re.I)
user.write_bytes(source)
print('IIS Express site:',site)
