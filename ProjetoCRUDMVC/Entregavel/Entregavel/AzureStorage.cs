using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Text;

namespace Azure {
	public static class AzureStorage {
		private static CloudBlobContainer BlobContainer(string pasta, string arquivo, string container, string azureConnectionString, ref string caminho) {
			CloudStorageAccount storageAccount = CloudStorageAccount.Parse(azureConnectionString);
			CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
			CloudBlobContainer c = blobClient.GetContainerReference(container);

			if (c == null)
				return null;

			if (arquivo != null) {
				arquivo = arquivo.Trim();

				if (arquivo.Length > 0 && arquivo[0] == '/')
					arquivo = arquivo.Substring(1);
			} else {
				arquivo = "";
			}

			if (string.IsNullOrWhiteSpace(pasta)) {
				caminho = arquivo;
			} else {
				if ((pasta = pasta.Trim())[0] == '/') pasta = pasta.Substring(1);
				caminho = (pasta.Length == 0 ? arquivo : (pasta[pasta.Length - 1] == '/' ? pasta + arquivo : pasta + "/" + arquivo));
			}

			return c;
		}

		private static CloudBlockBlob BlobReference(string pasta, string arquivo, string container, bool deveExistirBlob, string azureConnectionString) {
			string caminho = null;
			CloudBlobContainer c = BlobContainer(pasta, arquivo, container, azureConnectionString, ref caminho);

			if (c == null || string.IsNullOrWhiteSpace(caminho))
				return null;

			if (deveExistirBlob) {
				bool existe = false;
				var lista = c.ListBlobs(caminho);
				foreach (var item in lista) {
					CloudBlob blob = item as CloudBlob;
					if (blob != null) {
						if (string.Equals(blob.Name, caminho, StringComparison.InvariantCultureIgnoreCase)) {
							existe = true;
							break;
						}
					} else if (item.Uri.ToString().EndsWith(caminho, StringComparison.InvariantCultureIgnoreCase)) {
						existe = true;
						break;
					}
				}

				if (!existe)
					return null;
			}

			return c.GetBlockBlobReference(caminho);
		}

		public static string ContentType(string arquivo) {
			int i;
			if (string.IsNullOrWhiteSpace(arquivo) || (i = arquivo.LastIndexOf('.')) < 0)
				return "application/octet-stream";
			switch (arquivo.Substring(i + 1).ToLowerInvariant()) {
				case "jpg":
				case "jpeg":
					return "image/jpeg";
				case "png":
					return "image/png";
				case "gif":
					return "image/gif";
				case "pdf":
					return "application/pdf";
				case "zip":
					return "application/zip";
				case "doc":
					return "application/msword";
				case "docx":
					return "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
				case "xls":
					return "application/vnd.ms-excel";
				case "xlsx":
					return "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
				case "txt":
					return "text/plain";
				case "csv":
					return "text/csv";
			}
			return "application/octet-stream";
		}

		public static void Upload(string pasta, string arquivo, byte[] buffer, string container, int inicio = 0, int tamanho = -1, string azureConnectionString = null) {
			CloudBlockBlob blob = BlobReference(pasta, arquivo, container, false, azureConnectionString);
			if (blob == null)
				return;
			blob.Properties.ContentType = ContentType(arquivo);
			blob.UploadFromByteArray(buffer, inicio, tamanho < 0 ? buffer.Length : tamanho);
		}

		public static void Upload(string pasta, string arquivo, Stream stream, string container, string azureConnectionString = null) {
			CloudBlockBlob blob = BlobReference(pasta, arquivo, container, false, azureConnectionString);
			if (blob == null)
				return;
			blob.Properties.ContentType = ContentType(arquivo);
			blob.UploadFromStream(stream);
		}

		public static void Upload(string pasta, string arquivo, string caminhoArquivoLocal, string container, string azureConnectionString = null) {
			CloudBlockBlob blob = BlobReference(pasta, arquivo, container, false, azureConnectionString);
			if (blob == null)
				return;
			blob.Properties.ContentType = ContentType(arquivo);
			blob.UploadFromFile(caminhoArquivoLocal);
		}

		public static bool Existe(string pasta, string arquivo, string container, string azureConnectionString = null) {
			return (BlobReference(pasta, arquivo, container, true, azureConnectionString) != null);
		}

		public static void Delete(string pasta, string arquivo, string container, string azureConnectionString = null) {
			CloudBlockBlob blob = BlobReference(pasta, arquivo, container, true, azureConnectionString);
			if (blob == null)
				return;
			blob.DeleteIfExists(DeleteSnapshotsOption.IncludeSnapshots);
		}

		public static byte[] DownloadBytes(string pasta, string arquivo, string container, string azureConnectionString = null) {
			CloudBlockBlob blob = BlobReference(pasta, arquivo, container, true, azureConnectionString);
			if (blob == null)
				return null;
			MemoryStream stream = new MemoryStream(128 * 1024);
			try {
				blob.DownloadToStream(stream);
				if (stream.Length == 0) {
					stream.Dispose();
					return null;
				}
			} catch {
				stream.Dispose();
				throw;
			}
			byte[] buffer = stream.GetBuffer();
			Array.Resize(ref buffer, (int)stream.Length);
			return buffer;
		}

		public static MemoryStream DownloadStream(string pasta, string arquivo, string container, string azureConnectionString = null) {
			CloudBlockBlob blob = BlobReference(pasta, arquivo, container, true, azureConnectionString);
			if (blob == null)
				return null;
			MemoryStream stream = new MemoryStream(16 * 1024);
			try {
				blob.DownloadToStream(stream);
				if (stream.Length == 0) {
					stream.Dispose();
					return null;
				}
			} catch {
				stream.Dispose();
				throw;
			}
			stream.Position = 0;
			return stream;
		}

		public static IEnumerable<IListBlobItem> Listar(string pasta, string prefixoArquivo, string container, string azureConnectionString = null) {
			string prefixoCaminho = null;
			CloudBlobContainer c = BlobContainer(pasta, prefixoArquivo, container, azureConnectionString, ref prefixoCaminho);

			if (c == null)
				return null;

			return c.ListBlobs(prefixoCaminho);
		}
	}
}
