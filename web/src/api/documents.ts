import { apiDelete, apiDownload, apiGet, apiPost, apiPostForm, apiPut } from './client'
import type { DocumentDetailResponse, DocumentSummaryDto } from './types'

export function getDocuments(folderId: string | null): Promise<DocumentSummaryDto[]> {
    const query = folderId ? `?folderId=${folderId}` : ''
    return apiGet<DocumentSummaryDto[]>(`/api/documents${query}`)
}

export function getDocument(id: string): Promise<DocumentDetailResponse> {
    return apiGet<DocumentDetailResponse>(`/api/documents/${id}`)
}

export function uploadDocument(input: {
    file: File
    name?: string
    folderId: string | null
}): Promise<DocumentDetailResponse> {
    const form = new FormData()
    form.append('File', input.file)
    if (input.name) form.append('Name', input.name)
    if (input.folderId) form.append('FolderId', input.folderId)

    return apiPostForm<DocumentDetailResponse>('/api/documents', form)
}

export function updateDocument(
    id: string,
    input: { name: string; folderId: string | null },
): Promise<DocumentDetailResponse> {
    return apiPut<DocumentDetailResponse>(`/api/documents/${id}`, input)
}

// Re-runs the liblouis translation for a document — e.g. after a previous
// attempt failed, or just to force a fresh pass.
export function retranslateDocument(id: string): Promise<DocumentDetailResponse> {
    return apiPost<DocumentDetailResponse>(`/api/documents/${id}/translate`)
}

export function deleteDocument(id: string): Promise<void> {
    return apiDelete(`/api/documents/${id}`)
}

function triggerBlobDownload(blob: Blob, fileName: string): void {
    const url = URL.createObjectURL(blob)
    const link = document.createElement('a')
    link.href = url
    link.download = fileName
    document.body.appendChild(link)
    link.click()
    link.remove()
    URL.revokeObjectURL(url)
}

export async function downloadOriginal(id: string, fileName: string): Promise<void> {
    const blob = await apiDownload(`/api/documents/${id}/original`)
    triggerBlobDownload(blob, fileName)
}

export async function downloadTranslated(id: string, fileName: string): Promise<void> {
    const blob = await apiDownload(`/api/documents/${id}/translated`)
    triggerBlobDownload(blob, fileName)
}
