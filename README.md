# PerpetuaNet
Um aplicativo desktop com interface única, sincronização P2P via WebRTC e download de torrents via MonoTorrent.

## Funcionalidades
- Interface em uma única janela (`MainWindow`) com navegação por botões.
- Sincronização P2P usando WebRTC.NET.
- Download de torrents com MonoTorrent.

## Como executar
1. Clone o repositório: `git clone https://github.com/TheDequing/PerpetuaNet.git`
2. Restaure pacotes: `dotnet restore`
3. Compile e execute: `dotnet run`

## Dependências
- Avalonia 11.2.1
- CommunityToolkit.Mvvm 8.2.2
- WebRTCNET 1.75.0.1-Alpha
- MonoTorrent 2.0.7

## Projeto

Objetivo do Projeto:
Desenvolver um aplicativo que possua:

Sistema central com banco de dados.
Backup local criptografado em cada PC.
Funcionalidade de fallback com sincronização entre os PCs, garantindo a continuidade caso o sistema central falhe.
1. Arquitetura do Sistema
a. Sistema Central
Servidor Central: Hospedado na nuvem (AWS, Google Cloud, ou outro).
Banco de Dados Central: PostgreSQL ou MongoDB.
API RESTful: Para comunicação entre o app e o servidor central.
Funcionalidades do Sistema Central:
Gerenciamento de contas de usuário.
Armazenamento e sincronização de links e dados.
Notificações para usuários sobre falhas e manutenção.

b. Sistema Local (Fallback)
Banco de Dados Local: SQLite com SQLCipher (para criptografia).
Backup Local: Cada PC terá uma cópia criptografada do banco de dados.
Sincronização com Sistema Central: Sempre que o sistema central estiver acessível, os dados locais serão sincronizados com o servidor.

2. Funcionalidade de Fallback (Descentralização)
a. Comunicação entre PCs (P2P)
Tecnologia P2P: WebRTC ou libp2p para comunicação direta entre PCs.
Tabela de Nós: Cada PC manterá uma lista de nós (outros PCs) conhecidos.
Sincronização de Dados:
Quando um nó recebe uma atualização de banco de dados, ele propaga para outros nós.
Uso de CRDTs ou Timestamps para resolução de conflitos e consistência de dados.

b. Armazenamento e Criptografia
Criptografia Local: O banco de dados local será criptografado com SQLCipher.
Autenticação: Cada PC utilizará autenticação forte para acessar dados, incluindo login e senha.
Localização do Banco: O banco será salvo em locais seguros, como C:\ProgramData\ no Windows ou ~/.config/ no Linux, e o nome será ofuscado.

c. Detecção de Falhas
Monitoramento do Sistema Central:
Verificação contínua da conectividade com o servidor central.
Caso o servidor falhe, o sistema muda automaticamente para o modo offline e usa o banco de dados local.

Recuperação:
Quando a conexão for restaurada, os dados locais serão sincronizados com o servidor.

3. Funcionalidades de Sincronização
Modo Offline: O app funcionará normalmente usando dados locais caso o servidor central falhe.
Sincronização Automática:
Quando o servidor central estiver disponível novamente, as alterações do banco local serão enviadas.

A sincronização será feita em segundo plano, sem intervenção do usuário.
Resolução de Conflitos: Durante a sincronização, dados duplicados ou conflitantes serão resolvidos com base em timestamps.

4. Segurança
a. Criptografia de Dados
Criptografia de Trânsito: Toda comunicação entre o app e o servidor será protegida com TLS.
Criptografia Local: O banco de dados local será criptografado com AES-256.

b. Proteção contra Acesso Não Autorizado
Chave de Criptografia: Será armazenada de maneira segura (ex.: usando o Gerenciador de Senhas do Sistema Operacional).
Lista de Confiança: Conexões P2P só serão permitidas entre PCs autenticados.

5. Tecnologias e Ferramentas
Backend Central:
Servidor: Node.js ou Python (Django/FastAPI).
Banco de Dados: PostgreSQL ou MongoDB.
API: RESTful usando Express.js (Node.js) ou FastAPI (Python).
Aplicativo Local (Frontend/Backend Local):
Tecnologia de Aplicativo: Electron.js para desenvolvimento de apps desktop ou Python com Tkinter/PyQt.
Banco de Dados Local: SQLite com SQLCipher para criptografia.
Comunicação P2P: WebRTC ou libp2p.

Outras Ferramentas:
Sincronização: Gun.js ou OrbitDB para facilitar a sincronização entre PCs.
Segurança: Libs como crypto (Node.js) ou pycryptodome (Python) para criptografia.

6. Cronograma de Desenvolvimento
Fase 1: Planejamento e Arquitetura
Duração: 2 semanas
Definir a arquitetura detalhada do servidor central e do banco local.
Planejar a comunicação P2P entre os nós.
Escolher as ferramentas e tecnologias.
Fase 2: Desenvolvimento do Sistema Central
Duração: 3 semanas
Configurar o servidor central e o banco de dados.
Criar as APIs RESTful para sincronização de dados e comunicação com o app.
Implementar a lógica de autenticação e gerenciamento de usuários.
Fase 3: Desenvolvimento do Banco Local Criptografado
Duração: 2 semanas
Implementar o banco local com SQLite e criptografia SQLCipher.
Configurar backup local e lógica de fallback.
Fase 4: Implementação de Comunicação P2P
Duração: 3 semanas
Implementar a comunicação entre PCs usando WebRTC ou libp2p.
Desenvolver a lógica de sincronização e propagação de dados.
Testar a troca de dados entre dois ou mais PCs em rede.
Fase 5: Testes de Fallback e Sincronização
Duração: 2 semanas
Testar cenários de falha do servidor central e fallback para banco local.
Testar a sincronização de dados entre os PCs.
Resolver conflitos e ajustar a lógica de sincronização.
Fase 6: Implementação de Segurança e Backup
Duração: 2 semanas
Implementar criptografia de dados em trânsito e local.
Garantir segurança na comunicação entre os nós e autenticação.
Fase 7: Testes Finais e Ajustes
Duração: 2 semanas
Testar todos os casos de uso.
Ajustar bugs e melhorar a experiência do usuário.
Fase 8: Lançamento
Duração: 1 semana
Preparar o release final.
Criar o instalador e distribuir para os usuários.
7. Recursos e Custos
Servidores e Hospedagem: AWS, Google Cloud ou DigitalOcean para o servidor central.
Licenças e Ferramentas: Ferramentas de desenvolvimento, bibliotecas e frameworks.
Manutenção: Monitoramento do servidor, atualizações do app e gerenciamento de dados.




1. Funcionalidade de Download de Torrents
a. Escolha da Biblioteca para Torrent
LibTorrent (libtorrent-rasterbar): Biblioteca popular que pode ser embutida em aplicativos para gerenciar downloads de torrents.
Qbittorrent (Qt + libtorrent): Outra opção com uma interface gráfica simples e eficiente, também com base em libtorrent.
b. Implementação no App
Backend Local:

O app fará uso da biblioteca escolhida para iniciar, gerenciar e monitorar o download de torrents.
Cada link de torrent inserido no app será baixado usando essa biblioteca.
O download pode ser monitorado através da interface do app, com indicadores de progresso.
Funcionalidade Adicional:

O app permitirá que o usuário adicione links de torrents manualmente ou através de uma busca.
Poderá configurar opções como a pasta de download, número de conexões, limites de velocidade, etc.
c. Gerenciamento de Torrents
Monitoramento: Exibição de progresso (barra de progresso, % concluído, tempo restante).
Controle: Opções para pausar, continuar ou cancelar o download.
Armazenamento: Os arquivos baixados serão salvos no local especificado pelo usuário no app.
d. Interface do Usuário
Tela para gerenciar downloads, com a possibilidade de ver detalhes de cada torrent (tamanho, número de arquivos, etc).
O design pode incluir ícones para visualizar torrents ativos, pausados, ou completados.


2. Considerações Técnicas e Funcionais
a. Adição de Links de Torrent
O usuário poderá adicionar um link para o arquivo .torrent ou magnet link diretamente no aplicativo.
O aplicativo buscará o arquivo correspondente e começará o download imediatamente.

b. Integração com o Banco Local
Sincronização de Estado: O status de cada download será salvo no banco local (SQLite) para que o progresso seja preservado, mesmo após o app ser fechado e reaberto.
O banco de dados local pode armazenar:
Link do torrent.
Progresso de download.
Status do download (em andamento, pausado, concluído).

c. Criptografia de Dados
Segurança no Armazenamento: Os arquivos baixados podem ser criptografados localmente usando técnicas de criptografia, como AES, se necessário.
Criptografia de Trânsito: Toda comunicação com o servidor central e a rede P2P será protegida por TLS.


4. Recursos e Tecnologias
Frontend: O frontend pode ser feito com Electron.js ou PyQt, fornecendo uma interface gráfica para o gerenciamento de torrents.
Backend Local: O backend local pode ser desenvolvido com Python ou Node.js, integrando-se com a biblioteca de torrents.
Biblioteca de Torrent:
LibTorrent (para C++ e Python) ou qbittorrent (para C++/Qt).
Armazenamento Local: Banco de dados local para armazenar informações sobre os downloads em andamento e completos.

5. Cronograma de Implementação
Fase 1: Planejamento
Definir funcionalidades (quais opções serão disponibilizadas ao usuário).
Escolher a biblioteca de torrent a ser integrada.
Fase 2: Implementação do Download de Torrents
Integrar a biblioteca de torrent no app.
Criar a interface para gerenciamento de downloads.
Armazenar e monitorar o progresso no banco de dados local.
Fase 3: Testes
Testar o download e controle de torrents em diversos cenários (diferentes tipos de links e conexões).
Testar sincronização de status entre o servidor central e os bancos locais dos usuários.
Fase 4: Ajustes e Lançamento
Ajustar a interface e corrigir bugs encontrados.
Lançamento final e monitoramento de uso.