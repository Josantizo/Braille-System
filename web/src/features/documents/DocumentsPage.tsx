import { FolderPlus, Upload } from 'lucide-react'
import { useState } from 'react'
import { ApiError } from '../../api/client'
import Breadcrumb from './Breadcrumb'
import ConfirmDeleteDialog from './ConfirmDeleteDialog'
import DocumentRow from './DocumentRow'
import EmptyState from './EmptyState'
import FolderCard from './FolderCard'
import NameFormModal from './NameFormModal'
import {
    useCreateFolder,
    useDeleteDocument,
    useDeleteFolder,
    useDocuments,
    useFolder,
    useFolders,
    useRetranslateDocument,
    useUpdateDocument,
    useUpdateFolder,
    useUploadDocument,
} from './queries'
import UploadDocumentModal from './UploadDocumentModal'

type ModalState =
    | { type: 'createFolder' }
    | { type: 'renameFolder'; id: string; name: string }
    | { type: 'deleteFolder'; id: string; name: string }
    | { type: 'uploadDocument' }
    | { type: 'renameDocument'; id: string; name: string }
    | { type: 'deleteDocument'; id: string; name: string }
    | null

function errorMessageOf(error: unknown): string | null {
    if (!error) return null
    return error instanceof ApiError ? error.message : 'Ocurrió un error inesperado.'
}

export default function DocumentsPage() {
    const [currentFolderId, setCurrentFolderId] = useState<string | null>(null)
    const [modal, setModal] = useState<ModalState>(null)

    const folderDetail = useFolder(currentFolderId)
    const folders = useFolders(currentFolderId)
    const documents = useDocuments(currentFolderId)

    const createFolder = useCreateFolder(currentFolderId)
    const updateFolder = useUpdateFolder(currentFolderId)
    const deleteFolder = useDeleteFolder(currentFolderId)
    const uploadDocument = useUploadDocument(currentFolderId)
    const updateDocument = useUpdateDocument(currentFolderId)
    const deleteDocument = useDeleteDocument(currentFolderId)
    const retranslateDocument = useRetranslateDocument(currentFolderId)

    const isLoading = folders.isLoading || documents.isLoading
    const loadError = errorMessageOf(folders.error) ?? errorMessageOf(documents.error)
    const isEmpty = !isLoading && !loadError && folders.data?.length === 0 && documents.data?.length === 0

    function closeModal() {
        setModal(null)
    }

    return (
        <div className="mx-auto max-w-4xl">
            <div className="mb-4 flex flex-wrap items-center justify-between gap-3">
                <Breadcrumb items={folderDetail.data?.breadcrumb ?? []} onNavigate={setCurrentFolderId} />

                <div className="flex gap-2">
                    <button
                        type="button"
                        onClick={() => setModal({ type: 'createFolder' })}
                        className="flex items-center gap-1.5 rounded-lg border border-black/10 bg-white px-3 py-1.5 text-sm font-medium text-ink hover:bg-black/5"
                    >
                        <FolderPlus size={16} />
                        Nueva carpeta
                    </button>
                    <button
                        type="button"
                        onClick={() => setModal({ type: 'uploadDocument' })}
                        className="flex items-center gap-1.5 rounded-lg bg-accent px-3 py-1.5 text-sm font-medium text-white hover:bg-accent/90"
                    >
                        <Upload size={16} />
                        Subir libro
                    </button>
                </div>
            </div>

            {isLoading && <p className="text-sm text-muted">Cargando…</p>}

            {loadError && (
                <div className="rounded-lg border border-red-200 bg-red-50 p-4 text-sm text-red-700">
                    {loadError}
                </div>
            )}

            {isEmpty && <EmptyState />}

            {!isLoading && !loadError && (folders.data?.length ?? 0) > 0 && (
                <div className="mb-4 grid grid-cols-1 gap-2 sm:grid-cols-2 lg:grid-cols-3">
                    {folders.data!.map((folder) => (
                        <FolderCard
                            key={folder.id}
                            folder={folder}
                            onOpen={() => setCurrentFolderId(folder.id)}
                            onRename={() => setModal({ type: 'renameFolder', id: folder.id, name: folder.name })}
                            onDelete={() => setModal({ type: 'deleteFolder', id: folder.id, name: folder.name })}
                        />
                    ))}
                </div>
            )}

            {!isLoading && !loadError && (documents.data?.length ?? 0) > 0 && (
                <div className="flex flex-col gap-2">
                    {documents.data!.map((doc) => (
                        <DocumentRow
                            key={doc.id}
                            doc={doc}
                            onRename={() => setModal({ type: 'renameDocument', id: doc.id, name: doc.name })}
                            onDelete={() => setModal({ type: 'deleteDocument', id: doc.id, name: doc.name })}
                            onRetranslate={() => retranslateDocument.mutate(doc.id)}
                            isRetranslating={retranslateDocument.isPending && retranslateDocument.variables === doc.id}
                        />
                    ))}
                </div>
            )}

            {modal?.type === 'createFolder' && (
                <NameFormModal
                    title="Nueva carpeta"
                    submitLabel="Crear"
                    isSubmitting={createFolder.isPending}
                    errorMessage={errorMessageOf(createFolder.error)}
                    onClose={closeModal}
                    onSubmit={(name) => createFolder.mutate(name, { onSuccess: closeModal })}
                />
            )}

            {modal?.type === 'renameFolder' && (
                <NameFormModal
                    title="Renombrar carpeta"
                    initialName={modal.name}
                    submitLabel="Guardar"
                    isSubmitting={updateFolder.isPending}
                    errorMessage={errorMessageOf(updateFolder.error)}
                    onClose={closeModal}
                    onSubmit={(name) => updateFolder.mutate({ id: modal.id, name }, { onSuccess: closeModal })}
                />
            )}

            {modal?.type === 'deleteFolder' && (
                <ConfirmDeleteDialog
                    itemName={modal.name}
                    isSubmitting={deleteFolder.isPending}
                    errorMessage={errorMessageOf(deleteFolder.error)}
                    onClose={closeModal}
                    onConfirm={() => deleteFolder.mutate(modal.id, { onSuccess: closeModal })}
                />
            )}

            {modal?.type === 'uploadDocument' && (
                <UploadDocumentModal
                    isSubmitting={uploadDocument.isPending}
                    errorMessage={errorMessageOf(uploadDocument.error)}
                    onClose={closeModal}
                    onSubmit={(input) => uploadDocument.mutate(input, { onSuccess: closeModal })}
                />
            )}

            {modal?.type === 'renameDocument' && (
                <NameFormModal
                    title="Renombrar libro"
                    initialName={modal.name}
                    submitLabel="Guardar"
                    isSubmitting={updateDocument.isPending}
                    errorMessage={errorMessageOf(updateDocument.error)}
                    onClose={closeModal}
                    onSubmit={(name) => updateDocument.mutate({ id: modal.id, name }, { onSuccess: closeModal })}
                />
            )}

            {modal?.type === 'deleteDocument' && (
                <ConfirmDeleteDialog
                    itemName={modal.name}
                    isSubmitting={deleteDocument.isPending}
                    errorMessage={errorMessageOf(deleteDocument.error)}
                    onClose={closeModal}
                    onConfirm={() => deleteDocument.mutate(modal.id, { onSuccess: closeModal })}
                />
            )}
        </div>
    )
}
