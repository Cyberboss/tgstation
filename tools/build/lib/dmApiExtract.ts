import { unlink, mkdir, readdir, stat, rmdir } from "fs/promises";
import { join, dirname } from "path";

/**
 * Recursively deletes all contents of a directory
 */
const clearDirectory = async (dirPath) => {
  try {
    const stats = await stat(dirPath);
    if (stats.isDirectory()) {
      const files = await readdir(dirPath);
      await Promise.all(
        files.map(async (file) => {
          const filePath = join(dirPath, file);
          const fileStats = await stat(filePath);

          if (fileStats.isDirectory()) {
            await clearDirectory(filePath);
            await rmdir(filePath);
          } else {
            await unlink(filePath);
          }
        }),
      );
    }
  } catch (error) {
    if (error.code !== "ENOENT") {
      throw error;
    }
    // Directory doesn't exist, create it
    await mkdir(dirPath, { recursive: true });
  }
};

/**
 * Extracts a zip file using Bun's built-in zip support
 */
const extractZip = async (zipPath, outputPath) => {
  // Read the zip file
  const zipFile = Bun.file(zipPath);
  const zipArrayBuffer = await zipFile.arrayBuffer();

  // Use Bun's built-in zip extraction
  const zip = new Bun.unzip(zipArrayBuffer);

  for await (const [filename, content] of zip) {
    const fullPath = join(outputPath, filename);

    // Skip directories (they end with /)
    if (filename.endsWith("/")) {
      await mkdir(fullPath, { recursive: true });
      continue;
    }

    // Ensure parent directory exists
    await mkdir(dirname(fullPath), { recursive: true });

    // Write file content
    if (content instanceof Uint8Array) {
      await Bun.write(fullPath, content);
    } else {
      await Bun.write(fullPath, new Uint8Array(content));
    }
  }
};

/**
 * Main function to download and extract zip file
 */
const dmApiExtract = async (zipPath, outputPath) => {
  console.log(`Extracting zip file to: ${outputPath}`);

  await extractZip(zipPath, outputPath);

  console.log("Download and extraction completed successfully!");
};

export default dmApiExtract;
